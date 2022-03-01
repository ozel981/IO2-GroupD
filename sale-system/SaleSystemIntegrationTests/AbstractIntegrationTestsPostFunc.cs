using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SaleSystem;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Newsletters;
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
        protected async Task<HttpResponseMessage> PostNewComment(HttpClient client, int? authorId = null, int? parentPostId = null)
        {
            int postId = parentPostId == null ? await GetFirstPostId(client) : parentPostId.Value;
            var newComment = new NewComment
            {
                PostID = postId,
                Content = "New Comment"
            };

            int userId = authorId == null ? await GetFirstUserId(_client) : authorId.Value;
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newCommentJson = JsonConvert.SerializeObject(newComment);

            HttpContent content = new StringContent(newCommentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/comment", content);

            _client.DefaultRequestHeaders.Remove("userID");

            return response;
        }

        protected async Task<HttpResponseMessage> PostNewPost(HttpClient client, int? authorId = null)
        {
            var categorieId = await GetFirstCategorieId(client);
            var newPost = new PostCreate
            {
                CategoryID = categorieId,
                Title = "New post title",
                Content = "New post content"
            };

            int userId = authorId == null ? await GetFirstUserId(_client) : authorId.Value;
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newCommentJson = JsonConvert.SerializeObject(newPost);

            HttpContent content = new StringContent(newCommentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/post", content);

            _client.DefaultRequestHeaders.Remove("userID");

            return response;
        }

        protected async Task<HttpResponseMessage> PostNewAdmin(HttpClient client)
        {
            return await PostNewUser(client, true);
        }

        protected async Task<HttpResponseMessage> PostNewUser(HttpClient client, bool isAdmin = false)
        {
            var newUser = new UserUpdate
            {
                IsVerified = true,
                IsEntrepreneur = false,
                IsAdmin = isAdmin,
                IsActive = true,
                UserName = "New User",
                UserEmail = await GetUnoccupiedEmail(client)
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newUserJson = JsonConvert.SerializeObject(newUser);

            HttpContent content = new StringContent(newUserJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/users", content);
            _client.DefaultRequestHeaders.Remove("userID");

            return response;
        }

        protected async Task<HttpResponseMessage> PostSubscribeCategory(HttpClient client, int? authorId = null, int? subCategoryId = null)
        {
            int userId, categoryId;
            if (authorId == null)
            {
                (userId, categoryId) = await GetFirstUserWithUnsubscribedCategory(client);
            }
            else
            {
                if (subCategoryId == null)
                {
                    userId = authorId.Value;
                    categoryId = await GetFirstCommentId(client);
                }
                else
                {
                    userId = authorId.Value;
                    categoryId = subCategoryId.Value;
                }
            }

            var subInfo = new SubscriptionInfo
            {
                CategoryID = categoryId,
                IsSubscribed = true,
            };
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var subInfoJson = JsonConvert.SerializeObject(subInfo);

            HttpContent content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/newsletter", content);
            _client.DefaultRequestHeaders.Remove("userID");

            return response;
        }

        protected async Task<HttpResponseMessage> PutCommentLikeStatus(HttpClient client, bool like, int commentId, int? authorId = null)
        {
            var likeUpdate = new CommentLikeStatusUpdate
            {
                Like = like
            };

            int userId = authorId == null ? await GetFirstUserId(_client) : authorId.Value;
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var likeUpdateJson = JsonConvert.SerializeObject(likeUpdate);

            HttpContent content = new StringContent(likeUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/comment/" + commentId + "/likedUsers", content);
            _client.DefaultRequestHeaders.Remove("userID");

            return response;
        }

        protected async Task<HttpResponseMessage> PutPostLikeStatus(HttpClient client, bool like, int postId, int? authorId = null)
        {
            var postLike = new PostLikeStatusUpdate
            {
                Like = like,
            };

            int userId = authorId == null ? await GetFirstUserId(_client) : authorId.Value;
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var postLikeJson = JsonConvert.SerializeObject(postLike);

            HttpContent content = new StringContent(postLikeJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/post/" + postId + "/likedUsers", content);
            _client.DefaultRequestHeaders.Remove("userID");

            return response;
        }
    }
}
