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
using SaleSystem.Models.Users;
using SaleSystem.Models.Categories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SaleSystemUnitTests.ServicesTests
{
    public class UserServiceTest
    {
        private readonly SaleSystemDBSetFactory DBSetFactory;

        public UserServiceTest()
        {
            DBSetFactory = new SaleSystemDBSetFactory();
        }

        [Fact(DisplayName = "Get all users")]
        public void GetAllUsers()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            var users = service.GetAll();
            int usersCount = 0;

            List<string> resultUsersEmails = new List<string>();
            foreach (UserView u in users)
            {
                resultUsersEmails.Add(u.UserEmail);
                usersCount++;
            }
            Assert.Equal(DBusers.Count, usersCount);
            foreach (User user in DBusers)
            {
                Assert.Contains(user.EmailAddress, resultUsersEmails);
            }
            foreach (UserView user in users)
            {
                User DBuser = DBusers.Find((User u) => u.EmailAddress == user.UserEmail);
                Assert.Equal(DBuser.EmailAddress, user.UserEmail);
                Assert.Equal(DBuser.IsActive, user.IsActive);
                Assert.Equal(DBuser.Type == UserType.Admin, user.IsAdmin);
                Assert.Equal(DBuser.Type == UserType.Entrepreneur, user.IsEntrepreneur);
                Assert.Equal(DBuser.IsVerified, user.IsVerified);
                Assert.Equal(DBuser.Name, user.UserName);
            }
        }

        [Fact(DisplayName = "Get user that exists")]
        public void GetUserThatExists()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            foreach (User DBuser in DBusers)
            {
                var user = service.GetUser(DBuser.ID);
                Assert.Equal(DBuser.EmailAddress, user.UserEmail);
                Assert.Equal(DBuser.IsActive, user.IsActive);
                Assert.Equal(DBuser.Type == UserType.Admin, user.IsAdmin);
                Assert.Equal(DBuser.Type == UserType.Entrepreneur, user.IsEntrepreneur);
                Assert.Equal(DBuser.IsVerified, user.IsVerified);
                Assert.Equal(DBuser.Name, user.UserName);
            }
        }

        [Fact(DisplayName = "Get user that doesn't exist")]
        public void GetUserThatDoesntExist()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);

            Assert.Throws<Exception>(() => service.GetUser(-1));
        }

        [Fact(DisplayName = "Delete user by admin")]
        public void DeleteUserByAdmin()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = new Mock<DbSet<LikeComment>>();
            Mock<DbSet<LikePost>> mockLikePostsSet = new Mock<DbSet<LikePost>>();
            Mock<DbSet<Comment>> mockCommentsSet = new Mock<DbSet<Comment>>();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            mockContext.Setup(c => c.LikesUsersPosts).Returns(mockLikePostsSet.Object);
            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int adminID = DBusers.Where(u => u.Type == UserType.Admin).FirstOrDefault().ID;
            int userID = DBusers.Where(u => u.Type == UserType.Individual).FirstOrDefault().ID;

            service.DeleteUser(userID, adminID);

            mockUsersSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Delete user by user")]
        public void DeleteUserByUser()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<LikeComment>> mockLikeCommentsSet = new Mock<DbSet<LikeComment>>();
            Mock<DbSet<LikePost>> mockLikePostsSet = new Mock<DbSet<LikePost>>();
            Mock<DbSet<Comment>> mockCommentsSet = new Mock<DbSet<Comment>>();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.LikesUsersComments).Returns(mockLikeCommentsSet.Object);
            mockContext.Setup(c => c.LikesUsersPosts).Returns(mockLikePostsSet.Object);
            mockContext.Setup(c => c.Comments).Returns(mockCommentsSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int userID = DBusers.Where(u => u.Type == UserType.Individual).FirstOrDefault().ID;

            service.DeleteUser(userID, userID);

            mockUsersSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Delete user without permissions")]
        public void DeleteUserWithoutPermissions()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int enterpreneurID = DBusers.Where(u => u.Type == UserType.Entrepreneur).FirstOrDefault().ID;
            int userID = DBusers.Where(u => u.Type == UserType.Individual).FirstOrDefault().ID;

            Assert.Throws<UnauthorizedAccessException>(() => service.DeleteUser(enterpreneurID, userID));
        }

        [Fact(DisplayName = "Delete user that doesn't exist")]
        public void DeleteUserThatDoesntExist()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int adminID = DBusers.Where(u => u.Type == UserType.Admin).FirstOrDefault().ID;

            Assert.Throws<Exception>(() => service.DeleteUser(-1, adminID));
        }

        [Fact(DisplayName = "Update user by admin")]
        public void UpdateUserByAdmin()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int adminID = DBusers.Where(u => u.Type == UserType.Admin).FirstOrDefault().ID;
            int userID = DBusers.Where(u => u.Type == UserType.Individual).FirstOrDefault().ID;

            service.UpdateUser(userID, new UserUpdate
            {
                IsActive = false,
                IsAdmin = false,
                IsEntrepreneur = false,
                IsVerified = true,
                UserEmail = "new@mail.com"
            }, adminID);

            mockUsersSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Update user by user")]
        public void UpdateUserByUser()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int userID = DBusers.Where(u => u.Type == UserType.Individual).FirstOrDefault().ID;

            service.UpdateUser(userID, new UserUpdate
            {
                IsActive = false,
                IsAdmin = false,
                IsEntrepreneur = false,
                IsVerified = true,
                UserEmail = "new@mail.com"
            }, userID);

            mockUsersSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Update user without permissions")]
        public void UpdateUserWithoutPermissions()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int enterpreneurID = DBusers.Where(u => u.Type == UserType.Entrepreneur).FirstOrDefault().ID;
            int userID = DBusers.Where(u => u.Type == UserType.Individual).FirstOrDefault().ID;

            Assert.Throws<UnauthorizedAccessException>(() => service.UpdateUser(enterpreneurID, new UserUpdate
            {
                IsActive = true,
                IsAdmin = false,
                IsEntrepreneur = true,
                IsVerified = true,
                UserEmail = "new@mail.com"
            }, userID));
        }

        [Fact(DisplayName = "Update user that doesn't exist")]
        public void UpdateUserThatDoesntExist()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            int adminID = DBusers.Where(u => u.Type == UserType.Admin).FirstOrDefault().ID;

            Assert.Throws<Exception>(() => service.UpdateUser(-1, new UserUpdate
            {
                IsActive = true,
                IsAdmin = false,
                IsEntrepreneur = true,
                IsVerified = true,
                UserEmail = "new@mail.com"
            }, adminID));
        }

        [Fact(DisplayName = "Create user")]
        public void CreateUser()
        {
            ConnectionFactory factory = new ConnectionFactory();
            SaleSystemDBContext context = factory.CreateContextForSQLite();

            var newUser = new UserUpdate
            {
                IsActive = false,
                IsAdmin = false,
                IsEntrepreneur = false,
                IsVerified = false,
                UserEmail = "newuser@email.com",
                UserName = "New User"
            };

            UserService userService = new UserService(context);
            var response = userService.CreateUser(newUser);

            Assert.Equal(1, response.Id);
        }

        [Fact(DisplayName = "Create user that already exists")]
        public void CreateUserThatAlreadyExists()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();

            string userEmail = DBusers.FirstOrDefault().EmailAddress;

            Assert.Throws<Exception>(() => service.CreateUser(new UserUpdate
            {
                IsActive = false,
                IsAdmin = false,
                IsEntrepreneur = false,
                IsVerified = false,
                UserEmail = userEmail,
                UserName = "New User"
            }));
        }

        [Theory(DisplayName = "Get subscribed categories (include 0 subs and no user)")]
        [InlineData(2, 2)]
        [InlineData(1, 0)]
        [InlineData(3, 0)]
        public void GetSubscribedCategories(int userID, int subsCount)
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<User>> mockUsersSet = DBSetFactory.GetUsersMockDBSet();
            Mock<DbSet<SubscriberCategory>> mockSubscriberCategorySet = DBSetFactory.GetSubscriberCategoriesMockDBSet();

            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.SubscribersCategories).Returns(mockSubscriberCategorySet.Object);

            UserService service = new UserService(mockContext.Object);
            var DBusers = DBSetFactory.GetLastInitUsers();
            var DBsubscriberCategory = DBSetFactory.GetLastInitSubscriberCategories();

            var categories = service.GetSubscribedCategories(userID);
            int categoriesCount = 0;
            foreach (CategoryID category in categories)
            {
                Assert.Single(DBsubscriberCategory.Where(sc => sc.SubscriberID == userID && sc.CategoryID == category.ID).ToList());
                categoriesCount++;
            }
            Assert.Equal(subsCount, categoriesCount);
        }
    }
}
