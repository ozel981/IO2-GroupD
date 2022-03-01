using Microsoft.EntityFrameworkCore;
using Moq;
using MoqExpression;
using SaleSystem.Database.DatabaseModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SaleSystem.Services;
using SaleSystem.Database;
using SaleSystemUnitTests.MockData;
using SaleSystem.Models.Comments;
using System;

namespace SaleSystemUnitTests.ServicesTests
{
    public class CommentServiceTest
    {
        private readonly SaleSystemDBSetFactory DBSetFactory;

        public CommentServiceTest()
        {
            DBSetFactory = new SaleSystemDBSetFactory();
        }

        [Theory(DisplayName = "Return all of post's comments")]
        [InlineData(0, 2, new int[] { 0, 1 }, 1)]
        [InlineData(1, 1, new int[] { 2 }, 3)]
        [InlineData(1, 1, new int[] { 2 }, 2)]
        public void GetPostComments_ValidCall(int postID, int commentCount, int[] expectedUserIDs, int userID)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);
            CommentService service = new CommentService(mockContext.Object);
            var DBcomments = DBSetFactory.GetLastInitComments();

            var comments = service.GetPostComments(postID, userID);
            Assert.Equal(commentCount, comments.Count());
            foreach (int ID in expectedUserIDs)
            {
                Assert.Contains(comments, c => c.ID == ID);
            }
            foreach (CommentView comment in comments)
            {
                Comment DBcomment = DBcomments.Find((Comment c) => c.ID == comment.ID);
                Assert.Equal(DBcomment.Content, comment.Content);
                Assert.Equal(DBcomment.User.ID == userID, comment.OwnerMode);
                Assert.Equal(DBcomment.CreationDateTime, comment.Date);
                Assert.Equal(DBcomment.LikeComment.Any(c => c.UserID == userID), comment.IsLikedByUser);
                Assert.Equal(DBcomment.LikeComment.Count, comment.LikesCount);
            }
        }

        [Theory(DisplayName = "Return comment with comment ID")]
        [InlineData(0, 3)]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        public void GetComment_ValidCall(int commentID, int userID)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);
            var DBcomments = DBSetFactory.GetLastInitComments();

            var comment = service.GetComment(commentID, userID);

            Comment DBcomment = DBcomments.Find((Comment c) => c.Content == comment.Content);
            Assert.Equal(DBcomment.Content, comment.Content);
            Assert.Equal(DBcomment.CreationDateTime, comment.Date);
            Assert.Equal(DBcomment.LikeComment.Any(c => c.UserID == userID), comment.IsLikedByUser);
            Assert.Equal(DBcomment.LikeComment.Count, comment.LikesCount);
        }

        [Theory(DisplayName = "Return users liking comment")]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        public void GetUsersLikingComment_ValidCall(int commentID, int likesCount)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = DBSetFactory.GetLikeCommentsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            CommentService service = new CommentService(mockContext.Object);
            var DBcomments = DBSetFactory.GetLastInitComments();

            var users = service.GetUsersLikingComment(commentID);

            Assert.Equal(likesCount, users.Count());
            //TODO: add a content correctness when UserView gonna be ready
        }

        [Fact(DisplayName = "Return empty list if post has no comments")]
        public void GetPostComments_NoComments_ValidCall()
        {
            int userID = 1;
            int postID = 2;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);
            CommentService service = new CommentService(mockContext.Object);
            var DBcomments = DBSetFactory.GetLastInitComments();

            var comments = service.GetPostComments(postID, userID);
            Assert.Empty(comments);
        }

        [Fact(DisplayName = "Return all comments")]
        public void GetComments_ValidCall()
        {
            int userID = 2;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = DBSetFactory.GetLikeCommentsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            CommentService service = new CommentService(mockContext.Object);
            var DBcomments = DBSetFactory.GetLastInitComments().Where(c => c.User.ID == userID).ToList();

            var comments = service.GetComments(userID);

            List<int> resultIDs = new List<int>();
            comments.ToList().ForEach(c => resultIDs.Add(c.ID));
            Assert.Equal(DBcomments.Count, comments.Count());
            foreach (Comment comment in DBcomments)
            {
                Assert.Contains(comment.ID, resultIDs);
            }
            foreach (CommentView comment in comments)
            {
                Comment DBcomment = DBcomments.Find((Comment c) => c.ID == comment.ID);
                Assert.Equal(DBcomment.Content, comment.Content);
                Assert.Equal(DBcomment.User.ID == userID, comment.OwnerMode);
                Assert.Equal(DBcomment.CreationDateTime, comment.Date);
                Assert.Equal(DBcomment.LikeComment.Any(c => c.UserID == userID), comment.IsLikedByUser);
                Assert.Equal(DBcomment.LikeComment.Count, comment.LikesCount);
            }
        }

        [Fact(DisplayName = "Update comment's content")]
        public void UpdateComment_ValidCall()
        {
            int userID = 3;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            service.UpdateComment(new CommentUpdate
            {
                Content = "new content"
            }, 0, userID);

            mockCommentsSet.Verify(m => m.Update(It.IsAny<Comment>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Throw exeption if user has no permission to edit comment")]
        public void UpdateComment_NoPermissionForUser_InvalidCall()
        {
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<UnauthorizedAccessException>(() => service.UpdateComment(new CommentUpdate
            {
                Content = "new content"
            }, 0, userID));
        }

        [Fact(DisplayName = "Throw exception if comment not exists")]
        public void UpdateComment_CommentNotExists_InvalidCall()
        {
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<Exception>(() => service.UpdateComment(new CommentUpdate
            {
                Content = "new content"
            }, 10, userID));
        }

        [Theory(DisplayName = "Remove comment")]
        [InlineData(0, 3)]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        public void RemoveComment_ValidCall(int commentID, int userID)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            service.RemoveComment(commentID, userID);

            mockCommentsSet.Verify(m => m.Remove(It.IsAny<Comment>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Throw exception if comment not exists")]
        public void RemoveComment_CommentNotExists_InvalidCall()
        {
            int commentID = 10;
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
           .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<Exception>(() => service.RemoveComment(commentID, userID));
        }

        [Fact(DisplayName = "Throw exeption if user has no permission to remove comment")]
        public void RemoveComment_NoPermissionForUser_InvalidCall()
        {
            int commentID = 0;
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            mockCommentsSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitComments().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<UnauthorizedAccessException>(() => service.RemoveComment(commentID, userID));
        }

        [Theory(DisplayName = "Remove like from comment")]
        [InlineData(2, 0, false)]
        public void UpdateCommentLikeStatus_RemoveLike_ValidCall(int userID, int commentID, bool likeStatus)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = DBSetFactory.GetLikeCommentsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            service.UpdateCommentLikeStatus(commentID, new CommentLikeStatusUpdate
            {
                Like = likeStatus,
            }, userID);

            mockLikeCommentsSet.Verify(m => m.Remove(It.IsAny<LikeComment>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Theory(DisplayName = "Don't change anything if change status and status in db are same")]
        [InlineData(2, 0, true)]
        public void UpdateCommentLikeStatus_CommentAlreadyLiked_ValidCall(int userID, int commentID, bool likeStatus)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = DBSetFactory.GetLikeCommentsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            service.UpdateCommentLikeStatus(commentID, new CommentLikeStatusUpdate
            {
                Like = likeStatus,
            }, userID);

            mockLikeCommentsSet.Verify(m => m.Add(It.IsAny<LikeComment>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Theory(DisplayName = "Add like to comment")]
        [InlineData(3, 0, true)]
        public void UpdateCommentLikeStatus_ValidCall(int userID, int commentID, bool likeStatus)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = DBSetFactory.GetLikeCommentsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            service.UpdateCommentLikeStatus(commentID, new CommentLikeStatusUpdate
            {
                Like = likeStatus,
            }, userID);

            mockLikeCommentsSet.Verify(m => m.Add(It.IsAny<LikeComment>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Throw exception if comment not exists")]
        public void UpdateCommentLikeStatus_CommentNotExists_InvalidCall()
        {
            int commentID = 10;
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = DBSetFactory.GetLikeCommentsMockDBSet();

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<Exception>(() => service.UpdateCommentLikeStatus(commentID, new CommentLikeStatusUpdate
            {
                Like = true
            }, userID));
        }

        [Fact(DisplayName = "Add new comment")]
        public void AddComment_ValidCall()
        {
            ConnectionFactory factory = new ConnectionFactory();
            SaleSystemDBContext context = factory.CreateContextForSQLite();

            var user = new User
            {
                Name = "Name",
                EmailAddress = "Name@user.pl",
                Type = UserType.Individual,
                IsActive = true,
                IsVerified = true,
                CreationDateTime = DateTime.Now
            };
            context.Users.Add(user);
            context.SaveChanges();

            var category = new Category
            {
                Name = "RTV/AGD"
            };
            context.Categories.Add(category);
            context.SaveChanges();

            var post = new Post
            {
                Title = "Title",
                Content = "Content",
                CreationDateTime = DateTime.Now,
                Creator = user,
                Category = category
            };
            context.Posts.Add(post);
            context.SaveChanges();

            var newComment = new NewComment
            {
                PostID = 1,
                Content = "New comment"
            };

            CommentService commentService = new CommentService(context);
            var response = commentService.AddComment(newComment, 1);
            Assert.Equal(1, response.Id);
        }

        [Fact(DisplayName = "Throw exception if post not exists")]
        public void AddComment_PostWithPostIDNotExists_InvalidCall()
        {
            int userID = 3;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();
            mockPostsSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitPosts().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<Exception>(() => service.AddComment(new NewComment
            {
                PostID = 8,
                Content = "new comment"
            }, userID));
        }

        [Fact(DisplayName = "Throw exception if user not exists")]
        public void AddComment_UserWithUserIDNotExists_InvalidCall()
        {
            int userID = 10;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<DbSet<Comment>> mockCommentsSet = DBSetFactory.GetCommentsMockDBSet();
            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            mockUsersSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitUsers().FirstOrDefault(c => c.ID == (int)x[0]));
            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();
            mockPostsSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(x => DBSetFactory.GetLastInitPosts().FirstOrDefault(c => c.ID == (int)x[0]));

            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);
            CommentService service = new CommentService(mockContext.Object);

            Assert.Throws<Exception>(() => service.AddComment(new NewComment
            {
                PostID = 0,
                Content = "new comment"
            }, userID));
        }
    }
}
