using SaleSystem.Models.Comments;
using Xunit;

namespace SaleSystemUnitTests.ModelsTests.CommentModelTests
{
    public class NewCommentTest
    {
        [Theory]
        [InlineData(0, "test")]
        [InlineData(0, "")]
        [InlineData(1999, null)]
        public void ConstructorTest(int postID, string content)
        {
            NewComment newComment = new NewComment
            {
                PostID = postID,
                Content = content
            };
            Assert.Equal(postID, newComment.PostID);
            Assert.Equal(content, newComment.Content);
            Assert.NotNull(newComment);
        }
    }
}
