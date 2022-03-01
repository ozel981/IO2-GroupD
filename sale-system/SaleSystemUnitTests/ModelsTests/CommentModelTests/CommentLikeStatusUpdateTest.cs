using SaleSystem.Models.Comments;
using Xunit;

namespace SaleSystemUnitTests.ModelsTests.CommentModelTests
{
    public class CommentLikeStatusUpdateTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ConstructorTest(bool like)
        {
            CommentLikeStatusUpdate likeCommentUpdate = new CommentLikeStatusUpdate
            {
                Like = like
            };
            Assert.NotNull(likeCommentUpdate);
            Assert.Equal(like, likeCommentUpdate.Like);
        }
    }
}
