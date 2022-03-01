using SaleSystem.Models.Categories;
using SaleSystem.Models.Users;
using System.Collections.Generic;

namespace SaleSystem.Services
{
    public interface IUserService
    {
        public ResponseNewUser CreateUser(UserUpdate user);
        public void UpdateUser(int id, UserUpdate newUser, int permissionID);
        public void DeleteUser(int id, int permissionID);
        public UserView GetUser(int id);
        public IEnumerable<UserView> GetAll();
        public IEnumerable<CategoryID> GetSubscribedCategories(int userID);
    }
}
