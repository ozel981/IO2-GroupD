using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using System;
using Xunit;

namespace SaleSystemUnitTests.ModelsTests.CommentModelTests
{
    public class CommentViewTest
    {
        private readonly int commentID;
        private readonly bool ownerMode;
        private readonly UserView owner;
        private readonly DateTime dateTime;
        private readonly bool isLikedByUser;
        private readonly string content;
        private readonly int totalLikes;
        public CommentViewTest()
        {
            commentID = 0;
            ownerMode = false;
            owner = new UserView();
            dateTime = DateTime.Now;
            isLikedByUser = true;
            content = "test";
            totalLikes = 8;
        }

        [Fact]
        public void ConstructorTest()
        {

            CommentView commentView = new CommentView
            {
                ID = commentID,
                OwnerMode = ownerMode,
                AuthorName = owner.UserName,
                Date = dateTime,
                IsLikedByUser = isLikedByUser,
                Content = content,
                LikesCount = totalLikes
            };

            Assert.NotNull(commentView);
            Assert.Equal(commentID, commentView.ID);
            Assert.Equal(ownerMode, commentView.OwnerMode);
            Assert.Equal(owner.UserName, commentView.AuthorName);
            Assert.Equal(dateTime, commentView.Date);
            Assert.Equal(isLikedByUser, commentView.IsLikedByUser);
            Assert.Equal(content, commentView.Content);
            Assert.Equal(totalLikes, commentView.LikesCount);
        }
    }
}
