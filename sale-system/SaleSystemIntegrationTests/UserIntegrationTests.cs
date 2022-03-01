using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SaleSystem;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Newsletters;
using SaleSystem.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemIntegrationTests
{
    public class UserIntegrationTests : AbstractIntegrationTests
    {
        [Fact(DisplayName = "GET Users ValidCall")]
        public async Task Get_Users_ValidCall()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var response = await _client.GetAsync(client_url + "/users");

            stringResponse = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(users);
            Assert.True(users.Exists(user => user.ID == newUserId));
        }

        [Fact(DisplayName = "GET User ValidCall")]
        public async Task Get_User_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/users/" + userId);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserView>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(userId, user.ID);
        }

        [Fact(DisplayName = "GET User InvalidCall user not exists")]
        public async Task Get_User_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/users/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User ValidCall")]
        public async Task Put_User_ValidCall()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "updatedUser")
            };

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + newUserId, content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User ValidCall many dots")]
        public async Task Put_User_ValidCall_Dots()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "updatedUser", "@mini.pw.edu.pl")
            };

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + newUserId, content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidCall user not exists")]
        public async Task Put_User_InvalidCall_NotExists()
        {
            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "New User Name",
                UserEmail = await GetUnoccupiedEmail(_client, "updatedUser")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/-1", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidCall no permission")]
        public async Task Put_User_InvalidCall_NoPermission()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "New User Name",
                UserEmail = "new@email.xd"
            };

            int noPermissionsUserId = await GetFirstUserIdWithoutPermitions(_client, newUserId);
            _client.DefaultRequestHeaders.Add("userID", noPermissionsUserId.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + newUserId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData email already exists")]
        public async Task Put_User_InvalidCall_InvalidData_EmalAlreadyExists()
        {
            var user = await GetFirstUser(_client);
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = user.UserEmail
            };

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + newUserId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData email has no @")]
        public async Task Put_User_InvalidCall_InvalidData_InvalidEmail1()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "wrongmail.com")
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData email has no domain")]
        public async Task Put_User_InvalidCall_InvalidData_InvalidEmail2()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "@wrongmail")
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData email 2x @")]
        public async Task Put_User_InvalidCall_InvalidData_InvalidEmail3()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail@", "@wrongmail.com")
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData white spaces")]
        public async Task Put_User_InvalidCall_InvalidData_InvalidEmail4()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "@wro ngmail.com")
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData no prefix")]
        public async Task Put_User_InvalidCall_InvalidData_InvalidEmail5()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = "@wrongmail.com"
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData no sufix")]
        public async Task Put_User_InvalidCall_InvalidData_InvalidEmail6()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "@.com")
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData email is empty")]
        public async Task Put_User_InvalidCall_InvalidData_EmptyEmail()
        {
            var user = await GetFirstUser(_client);

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserName = "Updated User",
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + user, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT User InvalidData user name is empty")]
        public async Task Put_User_InvalidCall_InvalidData_EmptyUserName()
        {
            var user = await GetFirstUser(_client);
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var userUpdate = new UserView
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                UserEmail = await GetUnoccupiedEmail(_client)
            };

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var userUpdateJson = JsonConvert.SerializeObject(userUpdate);

            HttpContent content = new StringContent(userUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/users/" + newUserId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE User ValidCall")]
        public async Task Delete_User_ValidCall()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());

            var response = await _client.DeleteAsync(client_url + "/users/" + newUserId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE User InvalidCall user not exists")]
        public async Task Delete_User_InvalidCall_UserNotExists()
        {
            int userId = await GetFirstUserId(_client);

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.DeleteAsync(client_url + "/users/" + userId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE User InvalidCall no permission")]
        public async Task Delete_User_InvalidCall_NoPermission()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            int noPermissionsUserId = await GetFirstUserIdWithoutPermitions(_client, newUserId);
            _client.DefaultRequestHeaders.Add("userID", noPermissionsUserId.ToString());

            var response = await _client.DeleteAsync(client_url + "/users/" + newUserId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE User ValidCall by Admin")]
        public async Task Delete_User_ByAdmin_ValidCall()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            newUserResponse = await PostNewAdmin(_client);
            stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newAdminId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", newAdminId.ToString());

            var response = await _client.DeleteAsync(client_url + "/users/" + newUserId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE User InvalidCall deleting the not existing user")]
        public async Task Delete_User_InvalidCall_NotExists()
        {
            int userId = await GetFirstAdminId(_client);

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.DeleteAsync(client_url + "/users/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User ValidCall")]
        public async Task Post_User_ValidCall()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client)
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "POST User ValidCall email has many dots")]
        public async Task Post_User_ValidCall_ManyDots()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client, "newuser", "@mini.pw.edu.pl")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData user name is empty")]
        public async Task Post_User_InvalidCall_InvalidData_EmptyUserName()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserEmail = await GetUnoccupiedEmail(_client)
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData email is empty")]
        public async Task Post_User_InvalidCall_InvalidData_EmptyEmai()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData email has no @")]
        public async Task Post_User_InvalidCall_InvalidData_InvalidEmail1()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "wrongmail.com")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData email has no domain")]
        public async Task Post_User_InvalidCall_InvalidData_InvalidEmail2()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "@wrongmail")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData email 2x @")]
        public async Task Post_User_InvalidCall_InvalidData_InvalidEmail3()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "@wrongm@ail.com")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData white spaces")]
        public async Task Post_User_InvalidCall_InvalidData_InvalidEmail4()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client, "wron gmail", "@wrongmail.com")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData no sufix")]
        public async Task Post_User_InvalidCall_InvalidData_InvalidEmail5()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = "@wrongmail.com"
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData no sufix")]
        public async Task Post_User_InvalidCall_InvalidData_InvalidEmail6()
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = false,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(_client, "wrongmail", "@.com")
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST User InvalidData email already exists")]
        public async Task Post_User_InvalidCall_InvalidData_EmailAlreadyExists()
        {
            UserView user = await GetFirstUser(_client);

            var newUser = new UserUpdate
            {
                IsVerified = false,
                IsEntrepreneur = false,
                IsAdmin = true,
                IsActive = true,
                UserName = "New User",
                UserEmail = user.UserEmail
            };

            _client.DefaultRequestHeaders.Add("userID", user.ID.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);
            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(client_url + "/users", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET UserSubscriptions ValidCall")]
        public async Task Get_UserSubscriptions_ValidCall()
        {
            int userId, categoryId;
            (userId, categoryId) = await GetFirstUserWithUnsubscribedCategory(_client);

            await PostSubscribeCategory(_client, userId, categoryId);

            var response = await _client.GetAsync(client_url + "/users/" + userId + "/subscribedCategories");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(categories);
            Assert.True(categories.Exists(category => category.ID == categoryId));
        }

        [Fact(DisplayName = "GET UserSubscriptions ValidCall epmty list")]
        public async Task Get_UserSubscriptions_ValidCall_EmptyList()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var response = await _client.GetAsync(client_url + "/users/" + newUserId + "/subscribedCategories");

            stringResponse = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(categories);
        }

        [Fact(DisplayName = "GET UserSubscriptions InvalidCall user not exists")]
        public async Task Get_UserSubscriptions_InvalidCall_UserNotExists()
        {
            var response = await _client.GetAsync(client_url + "/users/-1/subscribedCategories");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
