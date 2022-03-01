using Microsoft.EntityFrameworkCore;
using SaleSystem.Models.Posts;
using SaleSystem.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using SaleSystem.Database.DatabaseModels;
using SendGrid.Helpers.Errors.Model;

namespace SaleSystem.Services
{
    public class PostService : IPostService
    {
        private readonly SaleSystemDBContext _context;
        public PostService(SaleSystemDBContext context)
        {
            this._context = context;
        }

        public ResponseNewPost CreatePost(PostCreate p, int userId)
        {
            var tx = _context.Database.BeginTransaction();
            ResponseNewPost responseNewPost;
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.ID == userId);
                if (user == null)
                    throw new InvalidOperationException("user does not exist");

                var category = _context.Categories.FirstOrDefault(c => c.ID == p.CategoryID);
                if (category == null)
                    throw new InvalidOperationException("category does not exist");

                var date = DateTime.Now;

                var dbPost = new Post
                {
                    Title = p.Title,
                    Content = p.Content,
                    CreationDateTime = date,
                    Creator = user,
                    Category = category,
                    Comments = null,
                };
                var response = _context.Posts.Add(dbPost);
                _context.SaveChanges();
                responseNewPost = new ResponseNewPost { Id = dbPost.ID };
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw new Exception($"creating a post failed");
            }
            return responseNewPost;
        }

        public void UpdatePost(int id, PostUpdate postUpdate, int userId)
        {
            Post post = _context.Posts.Include(p => p.Creator).FirstOrDefault(p => p.ID == id);
            if (post == null)
                throw new Exception($"post with id {id} not found");

            if (post.Creator.ID != userId)
                throw new UnauthorizedAccessException("Access denied");

            Category category = _context.Categories.FirstOrDefault(c => c.ID == postUpdate.CategoryID);
            if (category != null)
            {
                post.Category = category;
            }
            else
            {
                throw new Exception($"category with id {postUpdate.CategoryID} not found");
            }

            post.Content = postUpdate.Content;
            post.Title = postUpdate.Title;

            if (postUpdate.IsPromoted && (post.PromotionEndDateTime == null || post.PromotionEndDateTime < DateTime.Now))
            {
                post.PromotionEndDateTime = DateTime.Now.AddDays(7);
            }
            else if (!postUpdate.IsPromoted)
            {
                post.PromotionEndDateTime = null;
            }

            _context.Posts.Update(post);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("could not save changes to db");
            }
        }

        public void DeletePost(int id, int userId)
        {
            var requester = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (requester == null)
            {
                throw new Exception($"user with id {userId} not found");
            }

            var post = _context.Posts
                .Include(c => c.Creator)
                .FirstOrDefault(u => u.ID == id);

            if (post == null)
            {
                throw new Exception($"post with id {id} not found");
            }

            if (post.Creator.ID != userId && !requester.IsAdmin())
                throw new UnauthorizedAccessException($"Access denied");

            try
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Could not save changes to database");
            }
        }

        public PostView GetPost(int id, int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            var post = this._context.Posts
                .Include(c => c.Creator)
                .Include(c => c.Category)
                .Include(c => c.LikePost)
                .FirstOrDefault(c => c.ID == id);

            if (post == null)
                throw new Exception($"post with id {id} does not exist");

            return new PostView
            {
                ID = post.ID,
                Title = post.Title,
                Content = post.Content,
                DateTime = post.CreationDateTime,
                Category = post.Category.Name,
                IsPromoted = post.PromotionEndDateTime > DateTime.Now,
                AuthorName = post.Creator.Name,
                AuthorID = post.Creator.ID,
                LikesCount = post.LikePost.Count,
                IsLikedByUser = post.LikePost.Any(c => c.UserID == userId)
            };
        }

        public IEnumerable<PostView> GetUserPosts(int userId)
        {
            var user = _context.Users
                .Include(c => c.PostsCreated).ThenInclude(c => c.Category)
                .Include(c => c.PostsCreated).ThenInclude(c => c.LikePost)
                .Include(c => c.PostsCreated).ThenInclude(c => c.Comments).ThenInclude(c => c.User)
                .Include(c => c.PostsCreated).ThenInclude(c => c.Comments).ThenInclude(c => c.LikeComment)
                .OrderByDescending(c => c.CreationDateTime)
                .FirstOrDefault(c => c.ID == userId);

            if (user == null)
            {
                throw new Exception($"user with id {userId} not found");
            }

            var userPosts = user.PostsCreated.OrderByDescending(c => c.CreationDateTime);
            List<PostView> postViews = new List<PostView>();

            foreach (Post post in userPosts)
            {
                postViews.Add(new PostView
                {
                    ID = post.ID,
                    Title = post.Title,
                    Content = post.Content,
                    DateTime = post.CreationDateTime,
                    Category = post.Category.Name,
                    IsPromoted = post.PromotionEndDateTime > DateTime.Now,
                    AuthorName = user.Name,
                    AuthorID = user.ID,
                    LikesCount = post.LikePost.Count,
                    IsLikedByUser = post.LikePost.Any(c => c.UserID == userId)
                });
            }

            return postViews;
        }

        public void PromotePost(PostPromotion p, int userId)
        {
            if (p.Duration <= 0)
                throw new Exception("Duration must be a positive number of days");

            Post post = _context.Posts.Include(c => c.Creator).FirstOrDefault(c => c.ID == p.ID);

            if (post == null)
                throw new Exception($"post with id {p.ID} not found");

            if (post.Creator.ID != userId)
                throw new UnauthorizedAccessException("Access denied");

            if (post.PromotionEndDateTime > DateTime.Now)
                post.PromotionEndDateTime = post.PromotionEndDateTime.Value.AddDays(p.Duration);
            else
                post.PromotionEndDateTime = DateTime.Now.AddDays(p.Duration);

            _context.Posts.Update(post);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("could not save changes to db");
            }
        }

        public IEnumerable<PostLikerID> GetPostLikers(int id)
        {
            Post post = _context.Posts
                .Include(c => c.LikePost)
                .FirstOrDefault(x => x.ID == id);

            if (post == null)
            {
                throw new Exception($"post with id {id} does not exist");
            }

            List<PostLikerID> postLikers = new List<PostLikerID>();
            foreach (var lp in post.LikePost)
            {
                postLikers.Add(new PostLikerID
                {
                    ID = lp.UserID.Value
                });
            }

            return postLikers;
        }

        public void UpdatePostLikeStatus(int postId, PostLikeStatusUpdate likePostUpdate, int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException($"user with id {userId} not found");
            }
            var post = _context.Posts.Include(c => c.LikePost)
                .FirstOrDefault(p => (p.ID == postId));
            if (post == null)
                throw new Exception($"post with id {postId} not found");

            if (likePostUpdate.Like && !post.LikePost.Any(p => p.UserID == userId))
            {
                _context.LikesUsersPosts.Add(new LikePost
                {
                    PostID = postId,
                    UserID = userId,
                });
            }

            if (!likePostUpdate.Like && post.LikePost.Any(p => p.UserID == userId))
            {
                _context.LikesUsersPosts
                    .Remove(_context.LikesUsersPosts
                        .First(p => p.UserID == userId && p.PostID == postId));
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

        public IEnumerable<PostView> GetAll(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            var allPosts = this._context.Posts
                .Include(c => c.Creator)
                .Include(c => c.Category)
                .Include(c => c.LikePost)
                .AsQueryable();
            var dbPostsPromoted = allPosts
                .Where(c => c.PromotionEndDateTime != null && DateTime.Now <= c.PromotionEndDateTime)
                .OrderByDescending(c => c.CreationDateTime)
                .ToList();
            var dbPostsNotPromoted = allPosts
                .Where(c => c.PromotionEndDateTime == null || c.PromotionEndDateTime < DateTime.Now)
                .OrderByDescending(c => c.CreationDateTime)
                .ToList();
            var dbPosts = dbPostsPromoted.Concat(dbPostsNotPromoted);

            return ConvertToPostViewList(dbPosts, userId);
        }

        public IEnumerable<PostView> GetFilteredPosts(int userId, List<int> categoriesIDs)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            var allPosts = this._context.Posts
                .Include(c => c.Creator)
                .Include(c => c.Category)
                .Include(c => c.LikePost)
                .Where(c => categoriesIDs.Contains(c.Category.ID))
                .AsQueryable();
            var dbPostsPromoted = allPosts
                .Where(c => c.PromotionEndDateTime != null && DateTime.Now <= c.PromotionEndDateTime)
                .OrderByDescending(c => c.CreationDateTime)
                .ToList();
            var dbPostsNotPromoted = allPosts
                .Where(c => c.PromotionEndDateTime == null || c.PromotionEndDateTime < DateTime.Now)
                .OrderByDescending(c => c.CreationDateTime)
                .ToList();
            var dbPosts = dbPostsPromoted.Concat(dbPostsNotPromoted);

            return ConvertToPostViewList(dbPosts, userId);
        }

        public PostPagedList GetFilteredPosts(int userId, List<int> categoriesIDs, int page, int pageSize)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user == null)
                throw new InvalidOperationException("user does not exist");

            var allPosts = this._context.Posts
                .Include(c => c.Creator)
                .Include(c => c.Category)
                .Include(c => c.LikePost)
                .Where(c => categoriesIDs.Contains(c.Category.ID))
                .AsQueryable();
            var dbPostsPromoted = allPosts
                .Where(c => c.PromotionEndDateTime != null && DateTime.Now <= c.PromotionEndDateTime)
                .OrderByDescending(c => c.CreationDateTime)
                .ToList();
            var dbPostsNotPromoted = allPosts
                .Where(c => c.PromotionEndDateTime == null || c.PromotionEndDateTime < DateTime.Now)
                .OrderByDescending(c => c.CreationDateTime)
                .ToList();
            var dbPosts = dbPostsPromoted.Concat(dbPostsNotPromoted);

            var isLastPage = dbPosts.Count() <= page * pageSize;

            var dbPostsLimited = dbPosts.Skip((page - 1) * pageSize).Take(pageSize);

            return new PostPagedList { Posts = ConvertToPostViewList(dbPostsLimited, userId), IsLastPage = isLastPage };
        }

        private static List<PostView> ConvertToPostViewList(IEnumerable<Post> posts, int userId)
        {
            var postViews = new List<PostView>();
            foreach (var p in posts)
            {
                postViews.Add(new PostView
                {
                    ID = p.ID,
                    Title = p.Title,
                    Content = p.Content,
                    DateTime = p.CreationDateTime,
                    Category = p.Category.Name,
                    IsPromoted = p.PromotionEndDateTime > DateTime.Now,
                    AuthorName = p.Creator.Name,
                    AuthorID = p.Creator.ID,
                    LikesCount = p.LikePost.Count,
                    IsLikedByUser = p.LikePost.Any(c => c.UserID == userId)
                });
            }
            return postViews;
        }
    }
}
