using Newtonsoft.Json;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Posts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemIntegrationTests
{
    public class PostIntegrationTests : AbstractIntegrationTests
    {
        [Fact(DisplayName = "GET Post ValidCall")]
        public async Task Get_Post_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/post/" + newPostId);

            stringResponse = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<PostView>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(post);
            Assert.Equal(newPostId, post.ID);
        }

        [Fact(DisplayName = "GET Post InvalidCall post not exists")]
        public async Task Get_Post_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "post/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET Post InvalidCall user not exists")]
        public async Task Get_Post_InvalidCall_UserNotExists()
        {
            int postId = await GetFirstPostId(_client);

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.GetAsync(client_url + "post/" + postId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post ValidCall")]
        public async Task Put_Post_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                Content = "New Content",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = false
            };

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/" + newPostId, content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidCall no permission")]
        public async Task Put_Post_InvalidCall_NoPermission()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                Content = "New Content",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = false
            };

            var noPremissionsId = await GetFirstUserIdWithoutPermitions(_client, userId);
            _client.DefaultRequestHeaders.Add("userID", noPremissionsId.ToString());
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/" + newPostId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidCall post not exists")]
        public async Task Put_Post_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                Content = "New Content",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = false
            };

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/-1", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidCall user not exists")]
        public async Task Put_Post_InvalidCall_UserNotExists()
        {
            int postId = await GetFirstPostId(_client);

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                Content = "New Content",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = false
            };

            _client.DefaultRequestHeaders.Add("userID", "-1");
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/" + postId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidData category not exists")]
        public async Task Put_Post_InvalidCall_InvalidData_Category()
        {
            var post = await GetFirstPost(_client);

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                Content = "New Content",
                CategoryID = -1,
                IsPromoted = false
            };

            _client.DefaultRequestHeaders.Add("userID", post.AuthorID.ToString());
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/" + post.ID, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidData title is empty")]
        public async Task Put_Post_InvalidCall_InvalidData_Title()
        {
            var post = await GetFirstPost(_client);

            var postUpdate = new PostUpdate
            {
                Content = "New Content",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = false
            };

            _client.DefaultRequestHeaders.Add("userID", post.AuthorID.ToString());
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/" + post.ID, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidData content is empty")]
        public async Task Put_Post_InvalidCall_InvalidData_Content()
        {
            var post = await GetFirstPost(_client);

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = false
            };

            _client.DefaultRequestHeaders.Add("userID", post.AuthorID.ToString());
            var postUpdateJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "post/" + post.ID, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Post InvalidData content is empty")]
        public async Task Get_PostComments_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            newPostResponse = await PostNewComment(_client, userId, newPostId);
            stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var response = await _client.GetAsync(client_url + "/post/" + newPostId + "/comments");

            stringResponse = await response.Content.ReadAsStringAsync();
            List<CommentView> comments = JsonConvert.DeserializeObject<List<CommentView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(comments);
            Assert.True(comments.Exists(comment => comment.ID == newCommentId));
        }

        [Fact(DisplayName = "PUT PromotePost ValidCall")]
        public async Task Put_PromotePost_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var postUpdate = new PostUpdate
            {
                Title = "New Title",
                Content = "New Content",
                CategoryID = await GetFirstCategorieId(_client),
                IsPromoted = true
            };

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var postPromotionJson = JsonConvert.SerializeObject(postUpdate);

            HttpContent content = new StringContent(postPromotionJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/post/" + newPostId, content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Post ValidCall")]
        public async Task Delete_Post_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            ;
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.DeleteAsync(client_url + "/post/" + newPostId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Post ValidCall deleted by admin")]
        public async Task Delete_PostByAdmin_ValidCall()
        {
            var newPostResponse = await PostNewPost(_client);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            int userId = await GetFirstAdminId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.DeleteAsync(client_url + "/post/" + newPostId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Post InvalidCall post not exists")]
        public async Task Delete_Post_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.DeleteAsync(client_url + "/post/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Post InvalidCall no permission")]
        public async Task Delete_Post_InvalidCall_NoPermission()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var noPermissionId = await GetFirstUserIdWithoutPermitions(_client, userId);
            _client.DefaultRequestHeaders.Add("userID", noPermissionId.ToString());

            var response = await _client.DeleteAsync(client_url + "/post/" + newPostId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Post InvalidCall user not exists")]
        public async Task Delete_Post_InvalidCall_UserNotExist()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.DeleteAsync(client_url + "/post/" + newPostId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory(DisplayName = "PUT PostLikeStatus ValidCall set status")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Put_LikePost_ValidCall(bool like)
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var likersBefore = await GetPostLikers(_client, newPostId);

            var response = await PutPostLikeStatus(_client, like, newPostId, userId);

            var likersAfter = await GetPostLikers(_client, newPostId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(like, likersAfter.Count > likersBefore.Count);
        }

        [Fact(DisplayName = "PUT PostLikeStatus ValidCall unlike")]
        public async Task Put_UnlikePost_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            await PutPostLikeStatus(_client, true, newPostId, userId);

            var response = await PutPostLikeStatus(_client, false, newPostId, userId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory(DisplayName = "PUT PostLikeStatus ValidCall 2x same status")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Put_LikePost_ValidCall_SameStatus(bool like)
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var response = await PutPostLikeStatus(_client, like, newPostId, userId);

            var likersBefore = await GetPostLikers(_client, newPostId);

            response = await PutPostLikeStatus(_client, like, newPostId, userId);

            var likersAfter = await GetPostLikers(_client, newPostId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(likersAfter.Count == likersBefore.Count);
        }

        [Theory(DisplayName = "PUT PostLikeStatus InvalidCall user not exists")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Put_LikePost_InvalidCall_UserNotExists(bool like)
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var response = await PutPostLikeStatus(_client, like, newPostId, -1);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET PostLikers ValidCall")]
        public async Task Get_PostLikers_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            await PutPostLikeStatus(_client, true, newPostId, userId);

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var response = await _client.GetAsync(client_url + "/post/" + newPostId + "/likedUsers");

            stringResponse = await response.Content.ReadAsStringAsync();
            var likers = JsonConvert.DeserializeObject<List<PostLikerID>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(likers);
            Assert.True(likers.Exists(liker => liker.ID == userId));
        }

        [Fact(DisplayName = "GET PostLikers InvalidCall post nor exists")]
        public async Task Get_PostLikers_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/post/-1/likedUsers");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
