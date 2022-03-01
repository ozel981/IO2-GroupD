using SaleSystem.Models.Comments;
using Xunit;

namespace SaleSystemUnitTests.ModelsTests.CommentModelTests
{
    public class CommentUpdateTest
    {
        [Theory]
        [InlineData("new text")]
        [InlineData("")]
        public void ConstructorTest(string content)
        {
            CommentUpdate commentUpdate = new CommentUpdate
            {
                Content = content
            };
            Assert.NotNull(commentUpdate);
            Assert.Equal(content, commentUpdate.Content);
        }
    }
}
