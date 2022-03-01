using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SaleSystem.Controllers;
using SaleSystem.Database;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using SaleSystem.Services;
using SaleSystemUnitTests.MockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemUnitTests.ControllersTests
{
    public class CommentsControllerTest
    {
        [Fact(DisplayName = "Return Ok result object for get comments")]
        public void GetComments_ValidCall()
        {
            List<CommentView> comments = new List<CommentView>
            {
                new CommentView
                {
                    ID = 0,
                    OwnerMode = true,
                    Content = "text0",
                    AuthorID = 0,
                    AuthorName = "author0",
                    Date = DateTime.Now,
                    LikesCount = 3,
                    IsLikedByUser = false
                },
                new CommentView
                {
                    ID = 1,
                    OwnerMode = false,
                    Content = "text1",
                    AuthorID = 1,
                    AuthorName = "author1",
                    Date = DateTime.Now,
                    LikesCount = 1,
                    IsLikedByUser = true
                }
            };
            int userID = 0;

            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.GetComments(It.IsAny<int>()))
                .Returns<int>(x => comments);

            CommentsController controller = new CommentsController(mockService.Object);

            var result = controller.GetComments(userID);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact(DisplayName = "Return error message id no connection with DB")]
        public void GetComments_NoCennectionWithDatabes_Error()
        {

            int userID = 0;

            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.GetComments(It.IsAny<int>()))
                .Returns<int>((x) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentsController controller = new CommentsController(mockService.Object);

            var result = controller.GetComments(userID);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Get comment")]
        public void GetComment_VallidCall()
        {

            int userID = 0;
            int commentID = 0;

            GetComment comment = new GetComment
            {
                Content = "text0",
                Date = DateTime.Now,
                IsLikedByUser = false,
                AuthorID = 2,
                AuthorName = "",
                LikesCount = 3
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.GetComment(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((x, y) => { return comment; });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.GetComment(commentID, userID);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact(DisplayName = "Return error message if no connection with DB")]
        public void GetComment_NoCennectionWithDatabes_Error()
        {

            int userID = 0;
            int commentID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.GetComment(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((x, y) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.GetComment(commentID, userID);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Get users liking comment")]
        public void GetUsersLikingAComment_VallidCall()
        {
            int commentID = 0;
            List<LikerID> likers = new List<LikerID>();
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.GetUsersLikingComment(It.IsAny<int>()))
                .Returns<int>((x) => { return likers; });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.GetUsersLikingAComment(commentID);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact(DisplayName = "Return error message if no connection with DB")]
        public void GetUsersLikingAComment_NoCennectionWithDatabes_Error()
        {
            int commentID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.GetUsersLikingComment(It.IsAny<int>()))
                .Returns<int>((x) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.GetUsersLikingAComment(commentID);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Get comment")]
        public void AddNewComment_VallidCall()
        {
            int userID = 0;
            NewComment comment = new NewComment
            {
                Content = "text0",
                PostID = 0
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.AddComment(It.IsAny<NewComment>(), It.IsAny<int>()))
                .Callback<NewComment, int>((nc, id) => { });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.AddNewComment(userID, comment);

            Assert.IsType<OkObjectResult>(result);
            mockService.Verify(ms => ms.AddComment(It.IsAny<NewComment>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Return bad request if save changes returned error")]
        public void AddNewComment_UpdateException_Error()
        {
            int userID = 0;
            NewComment comment = new NewComment
            {
                Content = "text0",
                PostID = 0
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.AddComment(It.IsAny<NewComment>(), It.IsAny<int>()))
                .Callback<NewComment, int>((nc, id) => { throw new DbUpdateException("Unable to save changes."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.AddNewComment(userID, comment);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Return error message if no connection with DB")]
        public void AddNewComment_NoCennectionWithDatabes_Error()
        {
            int userID = 0;
            NewComment comment = new NewComment
            {
                Content = "text0",
                PostID = 0
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.AddComment(It.IsAny<NewComment>(), It.IsAny<int>()))
                .Callback<NewComment, int>((nc, id) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.AddNewComment(userID, comment);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Update comment")]
        public void UpdateComment_ValidCall()
        {
            int commentID = 0;
            int userID = 0;
            CommentUpdate comment = new CommentUpdate
            {
                Content = "newtext0"
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.UpdateComment(It.IsAny<CommentUpdate>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<CommentUpdate, int, int>((cu, comid, id) => { });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.UpdateComment(commentID, userID, comment);

            Assert.IsType<OkResult>(result);
            mockService.Verify(ms => ms.UpdateComment(It.IsAny<CommentUpdate>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Return bad request if save changes returned error")]
        public void UpdateComment_UpdateException_Error()
        {
            int commentID = 0;
            int userID = 0;
            CommentUpdate comment = new CommentUpdate
            {
                Content = "newtext0"
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.UpdateComment(It.IsAny<CommentUpdate>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<CommentUpdate, int, int>((cu, comid, id) => { throw new DbUpdateException("Unable to save changes."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.UpdateComment(commentID, userID, comment);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Return error message if no connection with DB")]
        public void UpdateComment_NoCennectionWithDatabes_Error()
        {
            int commentID = 0;
            int userID = 0;
            CommentUpdate comment = new CommentUpdate
            {
                Content = "newtext0"
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.UpdateComment(It.IsAny<CommentUpdate>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<CommentUpdate, int, int>((nc, comid, id) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.UpdateComment(commentID, userID, comment);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Update comment like status")]
        public void UpdateCommentLikeStatus_VallidCall()
        {
            int commentID = 0;
            int userID = 0;
            CommentLikeStatusUpdate comment = new CommentLikeStatusUpdate
            {
                Like = true,
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.UpdateCommentLikeStatus(It.IsAny<int>(), It.IsAny<CommentLikeStatusUpdate>(), It.IsAny<int>()))
                .Callback<int, CommentLikeStatusUpdate, int>((comid, clsu, id) => { });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.UpdateCommentLikeStatus(commentID, userID, comment);

            Assert.IsType<OkResult>(result);
            mockService.Verify(ms => ms.UpdateCommentLikeStatus(It.IsAny<int>(), It.IsAny<CommentLikeStatusUpdate>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Return bad request if save changes returned error")]
        public void UpdateCommentLikeStatus_UpdateException_Error()
        {
            int commentID = 0;
            int userID = 0;
            CommentLikeStatusUpdate comment = new CommentLikeStatusUpdate
            {
                Like = true,
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.UpdateCommentLikeStatus(It.IsAny<int>(), It.IsAny<CommentLikeStatusUpdate>(), It.IsAny<int>()))
                .Callback<int, CommentLikeStatusUpdate, int>((comid, clsu, id) => { throw new DbUpdateException("Unable to save changes."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.UpdateCommentLikeStatus(commentID, userID, comment);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Return error message if no connection with DB")]
        public void UpdateCommentLikeStatus_NoCennectionWithDatabes_Error()
        {
            int commentID = 0;
            int userID = 0;
            CommentLikeStatusUpdate comment = new CommentLikeStatusUpdate
            {
                Like = true,
            };
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.UpdateCommentLikeStatus(It.IsAny<int>(), It.IsAny<CommentLikeStatusUpdate>(), It.IsAny<int>()))
                .Callback<int, CommentLikeStatusUpdate, int>((comid, clsu, id) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.UpdateCommentLikeStatus(commentID, userID, comment);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Delete comment")]
        public void DeleteComment_VallidCall()
        {
            int commentID = 0;
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.RemoveComment(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((dc, id) => { });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.DeleteComment(commentID, userID);

            Assert.IsType<OkResult>(result);
            mockService.Verify(ms => ms.RemoveComment(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Return bad request if save changes returned error")]
        public void DeleteComment_UpdateException_Error()
        {
            int commentID = 0;
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.RemoveComment(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((dc, id) => { throw new DbUpdateException("Unable to save changes."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.DeleteComment(commentID, userID);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact(DisplayName = "Return error message if no connection with DB")]
        public void DeleteComment_NoCennectionWithDatabes_Error()
        {
            int commentID = 0;
            int userID = 0;
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<CommentService> mockService = new Mock<CommentService>(mockContext.Object);
            mockService.Setup(s => s.RemoveComment(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((dc, id) => { throw new ObjectDisposedException("No connection with the database."); });

            CommentController controller = new CommentController(mockService.Object);

            var result = controller.DeleteComment(commentID, userID);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }
    }
}
