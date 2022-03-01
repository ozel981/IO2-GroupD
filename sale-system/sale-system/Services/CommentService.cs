using Microsoft.EntityFrameworkCore;
using SaleSystem.Database;
using SaleSystem.Database.DatabaseModels;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using SaleSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Services
{
    public class CommentService : ICommentService
    {
        private readonly SaleSystemDBContext _context;
        public CommentService(SaleSystemDBContext context)
        {
            _context = context;
        }

        public virtual IEnumerable<CommentView> GetComments(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            var comments = _context.Comments.Include(c => c.LikeComment)
                            .Include(c => c.User)
                            .Where(c => c.User.ID == userId).AsQueryable();
            return ConvertToCommentView(comments, userId);
        }

        public virtual IEnumerable<CommentView> GetPostComments(int postId, int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            var post = _context.Posts.FirstOrDefault(p => p.ID == postId);
            if (post == null)
                throw new Exception($"post with id {postId} not found");

            var comments = _context.Comments.Include(c => c.LikeComment)
                                            .Include(c => c.User)
                                            .Where(c => c.PostID == postId)
                                            .AsQueryable();

            return ConvertToCommentView(comments, userId);
        }

        private static List<CommentView> ConvertToCommentView(IQueryable<Comment> comments, int userId)
        {
            List<CommentView> commentViews = new List<CommentView>();
            foreach (Comment comment in comments)
            {
                commentViews.Add(new CommentView
                {
                    ID = comment.ID,
                    OwnerMode = comment.User.ID == userId,
                    Content = comment.Content,
                    Date = comment.CreationDateTime,
                    AuthorID = comment.User.ID,
                    AuthorName = comment.User.Name,
                    LikesCount = comment.LikeComment.Count,
                    IsLikedByUser = comment.LikeComment.Any(cl => cl.UserID == userId)
                });
            }
            return commentViews;
        }

        public virtual GetComment GetComment(int commentID, int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            Comment comment = _context.Comments.Include(c => c.LikeComment).Include(c => c.User).FirstOrDefault(c => c.ID == commentID);
            if (comment == null)
                throw new Exception($"comment with id {commentID} not found");

            return new GetComment
            {
                Id = commentID,
                OwnerMode = comment.User.ID == userId,
                Content = comment.Content,
                Date = comment.CreationDateTime,
                AuthorID = comment.User.ID,
                AuthorName = comment.User.Name,
                IsLikedByUser = comment.LikeComment.Any(cl => cl.UserID == userId),
                LikesCount = comment.LikeComment.Count,
            };
        }

        public virtual IEnumerable<LikerID> GetUsersLikingComment(int commentId)
        {
            var comment = _context.Comments.Include(c => c.LikeComment).FirstOrDefault(c => c.ID == commentId);
            if (comment == null)
                throw new Exception($"comment with id {commentId} not found");

            List<LikerID> likersIDs = new List<LikerID>();
            foreach (LikeComment like in comment.LikeComment)
            {
                likersIDs.Add(new LikerID
                {
                    ID = like.UserID.Value
                });
            }
            return likersIDs;
        }

        public virtual ResponseNewComment AddComment(NewComment newComment, int userId)
        {
            Post post = _context.Posts.FirstOrDefault(p => p.ID == newComment.PostID);
            if (post == null)
                throw new Exception($"post with id {newComment.PostID} not found");
            User user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new Exception($"user with id {userId} not found");
            Comment comment = new Comment
            {
                Content = newComment.Content,
                CreationDateTime = DateTime.Now,
                LikeComment = new List<LikeComment>(),
                PostID = newComment.PostID,
                User = user
            };
            var response = _context.Comments.Add(comment);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("could not save changes to db");
            }
            return new ResponseNewComment { Id = comment.ID };
        }

        public virtual void RemoveComment(int commentID, int userId)
        {
            var requester = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (requester == null)
            {
                throw new Exception($"user with id {userId} not found");
            }

            Comment comment = _context.Comments.Include(c => c.User).FirstOrDefault(c => c.ID == commentID);
            if (comment == null)
                throw new Exception($"comment with id {commentID} not found");

            if (comment.User.ID != userId && !requester.IsAdmin())
                throw new UnauthorizedAccessException("Access denied");

            _context.Comments.Remove(comment);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("could not save changes to db");
            }
        }

        public virtual void UpdateComment(CommentUpdate commentUpdate, int commentID, int userID)
        {
            Comment comment = _context.Comments.Include(c => c.User).FirstOrDefault(c => c.ID == commentID);
            if (comment == null)
                throw new Exception($"comment with id {commentID} not found");

            if (comment.User.ID != userID)
                throw new UnauthorizedAccessException("Access denied");

            comment.Content = commentUpdate.Content;
            _context.Comments.Update(comment);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("could not save changes to db");
            }
        }

        public virtual void UpdateCommentLikeStatus(int commentId, CommentLikeStatusUpdate likeCommentUpdate, int userId)
        {
            Comment comment = _context.Comments.Include(c => c.LikeComment)
                                               .FirstOrDefault(c => (c.ID == commentId));
            if (comment == null)
                throw new Exception($"comment with id {commentId} not found");

            if (likeCommentUpdate.Like && !comment.LikeComment.Any(c => c.UserID == userId))
            {
                _context.LikesUsersComments.Add(new LikeComment
                {
                    CommentID = commentId,
                    UserID = userId,
                });
            }

            if (!likeCommentUpdate.Like && comment.LikeComment.Any(c => c.UserID == userId))
            {
                _context.LikesUsersComments
                    .Remove(_context.LikesUsersComments
                    .First(c => c.UserID == userId && c.CommentID == commentId));
            }

            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("could not save changes to db");
            }
        }
    }
}
