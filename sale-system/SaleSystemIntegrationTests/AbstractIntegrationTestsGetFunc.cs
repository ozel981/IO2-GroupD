using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SaleSystem;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Posts;
using SaleSystem.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaleSystemIntegrationTests
{
    public partial class AbstractIntegrationTests : IDisposable
    {
        protected async Task<List<CategoryView>> GetCategories(HttpClient client)
        {
            var response = await _client.GetAsync(client_url + "/categories");

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CategoryView>>(stringResponse);
        }
        protected async Task<CategoryView> GetFirstCategorie(HttpClient client)
        {
            var categories = await GetCategories(client);

            return categories.Count > 0 ? categories[0] : null;
        }
        protected async Task<int> GetFirstCategorieId(HttpClient client)
        {
            var categorie = await GetFirstCategorie(client);

            return categorie != null ? categorie.ID : -1;
        }

        protected async Task<(int userId, int categoryId)> GetFirstUserWithUnsubscribedCategory(HttpClient client)
        {
            var users = await GetUsers(client);
            foreach (var user in users)
            {
                int categoryID = await GetFirstUnsubscribedCategoryId(client, user.ID);
                if (categoryID != -1)
                {
                    return (user.ID, categoryID);
                }
            }
            return (-1, -1);
        }

        protected async Task<int> GetFirstUnsubscribedCategoryId(HttpClient client, int userId)
        {
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var response = await client.GetAsync(client_url + "/users/" + userId + "/subscribedCategories");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var subscribedCategoriesIds = JsonConvert.DeserializeObject<List<ResponseWithId>>(stringResponse);

            response = await client.GetAsync(client_url + "/categories");

            stringResponse = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryView>>(stringResponse);


            _client.DefaultRequestHeaders.Remove("userID");
            foreach (var categorie in categories)
            {
                if (!subscribedCategoriesIds.Any(c => c.Id == categorie.ID))
                {
                    return categorie.ID;
                }
            }
            return -1;
        }

        protected async Task<CommentView> GetFirstComment(HttpClient client)
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await client.GetAsync(client_url + "/comments");

            var stringResponse = await response.Content.ReadAsStringAsync();
            List<CommentView> comments = JsonConvert.DeserializeObject<List<CommentView>>(stringResponse);
            _client.DefaultRequestHeaders.Remove("userID");

            return comments.Count > 0 ? comments[0] : null;
        }

        protected async Task<int> GetFirstCommentId(HttpClient client)
        {
            var comment = await GetFirstComment(client);

            return comment != null ? comment.ID : -1;
        }

        protected async Task<PostView> GetFirstPostWithCondition(HttpClient client, Func<PostView, bool> condition)
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await client.GetAsync(client_url + "/posts");

            var stringResponse = await response.Content.ReadAsStringAsync();
            List<PostView> posts = JsonConvert.DeserializeObject<List<PostView>>(stringResponse);

            _client.DefaultRequestHeaders.Remove("userID");

            foreach (PostView post in posts)
            {
                if (condition(post))
                {
                    return post;
                }
            }
            return null;
        }

        protected async Task<List<LikerID>> GetPostLikers(HttpClient client, int postId)
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var response = await _client.GetAsync(client_url + "/post/" + postId + "/likedUsers");
            _client.DefaultRequestHeaders.Remove("userID");

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<LikerID>>(stringResponse);
        }

        protected async Task<List<LikerID>> GetCommentLikers(HttpClient client, int commentId)
        {
            var response = await client.GetAsync(client_url + "/comment/" + commentId + "/likedUsers");

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<LikerID>>(stringResponse);
        }

        protected async Task<PostView> GetFirstPost(HttpClient client)
        {
            return await GetFirstPostWithCondition(client, (post) => true);
        }

        protected async Task<int> GetFirstPostId(HttpClient client)
        {
            var post = await GetFirstPost(client);
            return post != null ? post.ID : -1;
        }

        protected async Task<int> GetFirstUserIdWithoutPermitions(HttpClient client, int authorId)
        {
            List<UserView> users = await GetUsers(client);

            foreach (var user in users)
            {
                if (user.ID != authorId && !user.IsAdmin)
                {
                    return user.ID;
                }
            }
            return -1;
        }

        protected async Task<List<UserView>> GetUsers(HttpClient client)
        {
            var response = await client.GetAsync(client_url + "/users");

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<UserView>>(stringResponse);
        }

        protected async Task<UserView> GetFirstUser(HttpClient client)
        {
            List<UserView> users = await GetUsers(client);

            return users.Count > 0 ? users[0] : null;
        }

        protected async Task<int> GetFirstUserId(HttpClient client)
        {
            List<UserView> users = await GetUsers(client);

            return users.Count > 0 ? users[0].ID : -1;
        }

        protected async Task<int> GetFirstAdminId(HttpClient client)
        {
            List<UserView> users = await GetUsers(client);

            foreach (UserView user in users)
            {
                if (user.IsAdmin)
                {
                    return user.ID;
                }
            }

            return -1;
        }
    }
}
