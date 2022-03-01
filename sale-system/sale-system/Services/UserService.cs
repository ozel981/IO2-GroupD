using Microsoft.EntityFrameworkCore;
using SaleSystem.Database;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaleSystem.Services
{
    public class UserService : IUserService
    {
        private readonly SaleSystemDBContext _context;
        public UserService(SaleSystemDBContext context)
        {
            _context = context;
        }
        public ResponseNewUser CreateUser(UserUpdate user)
        {
            if (_context.Users.Where(u => u.EmailAddress == user.UserEmail).Any())
            {
                throw new Exception($"User with email: {user.UserEmail} already exists");
            }

            var newUser = CreateDatabaseUserFromUserModel(user);

            var response = _context.Users.Add(newUser);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Could not save changes to database");
            }

            return new ResponseNewUser { Id = newUser.ID };
        }

        public void UpdateUser(int id, UserUpdate newUser, int editorID)
        {
            var updatedUser = _context.Users.Where(u => u.ID == id).FirstOrDefault();
            var editorUser = _context.Users.Where(u => u.ID == editorID).FirstOrDefault();
            if (updatedUser == null)
            {
                throw new Exception($"User with id: {id} not found");
            }
            if (!editorUser.IsAdmin() && id != editorID)
                throw new UnauthorizedAccessException("Access denied");

            updatedUser.IsVerified = newUser.IsVerified;
            updatedUser.IsActive = newUser.IsActive;
            updatedUser.Name = newUser.UserName;
            updatedUser.EmailAddress = newUser.UserEmail;

            if (newUser.IsAdmin)
                updatedUser.Type = Database.DatabaseModels.UserType.Admin;
            else if (newUser.IsEntrepreneur)
                updatedUser.Type = Database.DatabaseModels.UserType.Entrepreneur;
            else
                updatedUser.Type = Database.DatabaseModels.UserType.Individual;

            _context.Users.Update(updatedUser);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Could not save changes to database");
            }
        }

        public void DeleteUser(int id, int editorID)
        {
            var user = _context.Users
                            .Include(c => c.Comments)
                            .Include(c => c.PostsCreated)
                            .Include(c => c.LikeComment)
                            .Include(c => c.LikePost)
                            .Where(u => u.ID == id)
                            .FirstOrDefault();
            var editorUser = _context.Users.Where(u => u.ID == editorID).FirstOrDefault();
            if (user == null)
            {
                throw new Exception($"User with id: {id} not found");
            }
            if (!editorUser.IsAdmin() && id != editorID)
                throw new UnauthorizedAccessException("Access denied");

            _context.LikesUsersComments.RemoveRange(user.LikeComment.ToList());
            _context.LikesUsersPosts.RemoveRange(user.LikePost.ToList());
            _context.Comments.RemoveRange(user.Comments.ToList());
            _context.Users.Remove(user);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Could not save changes to database");
            }
        }

        public UserView GetUser(int id)
        {
            var user = _context.Users.Where(u => u.ID == id).FirstOrDefault();
            if (user == null)
            {
                throw new Exception($"User with id: {id} not found");
            }

            return CreateUserFromDatabaseModel(user);
        }

        public IEnumerable<UserView> GetAll()
        {
            var usersContext = _context.Users.AsQueryable();

            List<UserView> usersList = new List<UserView>();
            foreach (Database.DatabaseModels.User user in usersContext)
            {
                usersList.Add(CreateUserFromDatabaseModel(user));
            }

            return usersList;
        }

        public IEnumerable<CategoryID> GetSubscribedCategories(int userID)
        {
            var user = _context.Users.Where(u => u.ID == userID).FirstOrDefault();
            if (user == null)
            {
                throw new Exception($"User with id: {userID} not found");
            }

            var subscribersCategories = _context.SubscribersCategories
                .Where(sc => sc.SubscriberID == userID)
                .Include(sc => sc.Category);

            List<CategoryID> categories = new List<CategoryID>();
            foreach (var sc in subscribersCategories)
            {
                categories.Add(new CategoryID
                {
                    ID = sc.CategoryID,
                });
            }

            return categories;
        }

        private static UserView CreateUserFromDatabaseModel(Database.DatabaseModels.User user)
        {
            return new UserView
            {
                ID = user.ID,
                UserName = user.Name,
                UserEmail = user.EmailAddress,
                IsAdmin = user.IsAdmin(),
                IsEntrepreneur = user.IsEntrepreneur(),
                IsActive = user.IsActive,
                IsVerified = user.IsVerified
            };
        }

        private static Database.DatabaseModels.User CreateDatabaseUserFromUserModel(UserUpdate user)
        {
            Database.DatabaseModels.UserType userType;
            if (user.IsAdmin)
                userType = Database.DatabaseModels.UserType.Admin;
            else if (user.IsEntrepreneur)
                userType = Database.DatabaseModels.UserType.Entrepreneur;
            else
                userType = Database.DatabaseModels.UserType.Individual;

            return new Database.DatabaseModels.User
            {
                Name = user.UserName,
                EmailAddress = user.UserEmail,
                Type = userType,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified
            };
        }
    }
}
