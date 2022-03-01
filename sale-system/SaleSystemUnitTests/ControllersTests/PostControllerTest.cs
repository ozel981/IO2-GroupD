using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SaleSystem.Controllers;
using SaleSystem.Database;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Posts;
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
    public class PostControllerTest
    {
        private SaleSystemDBSetFactory DBSetFactory;

        public PostControllerTest()
        {
            DBSetFactory = new SaleSystemDBSetFactory();
        }

        [Fact(DisplayName = "Return Ok result object for get comments")]
        public void GetPostComments_ValidCall()
        {
            List<CommentView> comments = new List<CommentView>
            {
                new CommentView
                {
                    ID = 0,
                    Content = "text0",
                    Date = DateTime.Now,
                    IsLikedByUser = false,
                    AuthorID = 0,
                    AuthorName = "author0",
                    OwnerMode = true,
                    LikesCount = 3
                },
                new CommentView
                {
                    ID = 1,
                    Content = "text1",
                    Date = DateTime.Now,
                    IsLikedByUser = true,
                    AuthorID = 1,
                    AuthorName = "author1",
                    OwnerMode = false,
                    LikesCount = 1,
                }
            };
            int userId = 0;
            int postId = 0;

            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<NewsletterService> mockNewsletterService = new Mock<NewsletterService>(mockContext.Object, null);
            Mock<PostService> mockPostService = new Mock<PostService>(mockContext.Object);
            Mock<CommentService> mockCommentService = new Mock<CommentService>(mockContext.Object);
            mockCommentService.Setup(s => s.GetPostComments(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((x, y) => comments);

            PostController controller = new PostController(mockPostService.Object,
                mockNewsletterService.Object, mockCommentService.Object);

            var result = controller.GetPostComments(userId, postId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact(DisplayName = "Return error message if bad request")]
        public void GetPostComments_BadRequest_Error()
        {

            int userId = -1;
            int postId = -1;

            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();
            Mock<NewsletterService> mockNewsletterService = new Mock<NewsletterService>(mockContext.Object, null);
            Mock<PostService> mockPostService = new Mock<PostService>(mockContext.Object);
            Mock<CommentService> mockCommentService = new Mock<CommentService>(mockContext.Object);
            mockCommentService.Setup(s => s.GetPostComments(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((x, y) => { throw new NullReferenceException($"Post with id {-1} not exists"); });

            PostController controller = new PostController(mockPostService.Object,
                mockNewsletterService.Object, mockCommentService.Object);

            var result = controller.GetPostComments(userId, postId);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }
    }
}
