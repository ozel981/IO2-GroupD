using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SaleSystem;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Posts;
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
    public class CommentIntegrationTests : AbstractIntegrationTests
    {
        [Fact(DisplayName = "GET Comments ValidCall")]
        public async Task Get_Comments_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/comments");

            stringResponse = await response.Content.ReadAsStringAsync();
            var comments = JsonConvert.DeserializeObject<List<CommentView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(comments);
            Assert.True(comments.Exists(comment => comment.ID == newCommentId));
        }

        [Fact(DisplayName = "GET Comments InvalidCall user not exists")]
        public async Task Get_Comments_InvalidCall_UserNotExists()
        {
            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.GetAsync(client_url + "/comments");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET Comment ValidCall")]
        public async Task Get_Comment_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/comment/" + newCommentId);

            stringResponse = await response.Content.ReadAsStringAsync();
            var responseComment = JsonConvert.DeserializeObject<GetComment>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseComment);
            Assert.Equal(newCommentId, responseComment.Id);
        }


        [Fact(DisplayName = "GET Comment InvalidCall comment not exists")]
        public async Task Get_Comment_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/comment/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET Comment InvalidCall user not exists")]
        public async Task Get_Comment_InvalidCall_UserNotExists()
        {
            int commentId = await GetFirstCommentId(_client);

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.GetAsync(client_url + "/comment/" + commentId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Comment ValidCall")]
        public async Task Put_Comment_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var commentUpdate = new CommentUpdate
            {
                Content = "New Content"
            };

            var commentUpdateJson = JsonConvert.SerializeObject(commentUpdate);

            HttpContent content = new StringContent(commentUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/comment/" + newCommentId, content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Comment InvalidCall comment not exists")]
        public async Task Put_Comment_InvalidCall_NotExists()
        {
            var commentUpdate = new CommentUpdate
            {
                Content = "New Content"
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var commentUpdateJson = JsonConvert.SerializeObject(commentUpdate);

            HttpContent content = new StringContent(commentUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/comment/-1", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Comment InvalidCall user not exists")]
        public async Task Put_Comment_InvalidCall_UserNotExists()
        {
            int commentId = await GetFirstCommentId(_client);

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var commentUpdate = new CommentUpdate
            {
                Content = "New Content"
            };

            var commentUpdateJson = JsonConvert.SerializeObject(commentUpdate);

            HttpContent content = new StringContent(commentUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/comment/" + commentId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Comment InvalidCall no permissions")]
        public async Task Put_Comment_InvalidCall_NoPermission()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            var noPermissionId = await GetFirstUserIdWithoutPermitions(_client, userId);

            _client.DefaultRequestHeaders.Add("userID", noPermissionId.ToString());

            var commentUpdate = new CommentUpdate
            {
                Content = "New Content"
            };

            var commentUpdateJson = JsonConvert.SerializeObject(commentUpdate);

            HttpContent content = new StringContent(commentUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/comment/" + newCommentId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT Comment InvalidData content is empty")]
        public async Task Put_Comment_InvalidCall_InvalidData()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var comment = new CommentView();
            var commentUpdateJson = JsonConvert.SerializeObject(comment);

            HttpContent content = new StringContent(commentUpdateJson, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(client_url + "/comment/" + newCommentId, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Comment ValidCall")]
        public async Task Delete_Comment_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.DeleteAsync(client_url + "/comment/" + newCommentId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Comment InvalidCall comment not exists")]
        public async Task Delete_Comment_InvalidCall_NotExists()
        {
            int userId = await GetFirstUserId(_client);

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.DeleteAsync(client_url + "/comment/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Comment InvalidCall no permission")]
        public async Task Delete_Comment_InvalidCall_NoPermission()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            var noPermissionId = await GetFirstUserIdWithoutPermitions(_client, userId);

            _client.DefaultRequestHeaders.Add("userID", noPermissionId.ToString());

            var response = await _client.DeleteAsync(client_url + "/comment/" + newCommentId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE Comment InvalidCall user not exists")]
        public async Task Delete_Comment_InvalidCall_UserNotExists()
        {
            int commentId = await GetFirstCommentId(_client);

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.DeleteAsync(client_url + "/comment/" + commentId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST Comment ValidCall")]
        public async Task Post_Comment_ValidCall()
        {
            var response = await PostNewComment(_client);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "POST Comment InvalidCall post not exists")]
        public async Task Post_Comment_InvalidCall_PostNotExists()
        {
            var newComment = new NewComment
            {
                PostID = -1,
                Content = "New Comment"
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var newCommentJson = JsonConvert.SerializeObject(newComment);

            HttpContent content = new StringContent(newCommentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/comment", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST Comment InvalidData content is empty")]
        public async Task Post_Comment_InvalidCall_InvalidData()
        {
            var newComment = new NewComment
            {
                PostID = await GetFirstPostId(_client)
            };

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());
            var newCommentJson = JsonConvert.SerializeObject(newComment);

            HttpContent content = new StringContent(newCommentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/comment", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST Comment InvalidCall user not exists")]
        public async Task Post_Comment_InvalidCall_UserNotExists()
        {
            var newComment = new NewComment
            {
                PostID = await GetFirstPostId(_client),
                Content = "New Comment"
            };

            _client.DefaultRequestHeaders.Add("userID", "-1");
            var newCommentJson = JsonConvert.SerializeObject(newComment);

            HttpContent content = new StringContent(newCommentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/comment", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET CommentLikers ValidCall empty list")]
        public async Task Get_EmptyCommentLikers_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var response = await _client.GetAsync(client_url + "/comment/" + newCommentId + "/likedUsers");

            stringResponse = await response.Content.ReadAsStringAsync();
            var likers = JsonConvert.DeserializeObject<List<LikerID>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(likers);
        }

        [Fact(DisplayName = "GET CommentLikers ValidCall not empty list")]
        public async Task Get_CommentLikers_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            var response = await PutCommentLikeStatus(_client, true, newCommentId, userId);

            var likers = await GetCommentLikers(_client, newCommentId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(likers);
            Assert.True(likers.Exists(liker => liker.ID == userId));
        }

        [Fact(DisplayName = "GET CommentLikers ValidCall")]
        public async Task Get_CommentLikers_InvalidCall_NotExists()
        {
            var response = await _client.GetAsync(client_url + "/comment/-1/likedUsers");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory(DisplayName = "PUT CommentLikeStatus ValidCall set status")]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Put_CommentLikeStatus_ValidCall(bool like)
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var likersBefore = await GetCommentLikers(_client, newCommentId);

            var response = await PutCommentLikeStatus(_client, like, newCommentId, userId);

            var likersAfter = await GetCommentLikers(_client, newCommentId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(like, likersAfter.Count > likersBefore.Count);
        }

        [Fact(DisplayName = "PUT CommentLikeStatus ValidCall unlike")]
        public async Task Put_CommentUnlike_ValidCall()
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            var response = await PutCommentLikeStatus(_client, true, newCommentId, userId);

            var likersBefore = await GetCommentLikers(_client, newCommentId);

            response = await PutCommentLikeStatus(_client, false, newCommentId, userId);

            var likersAfter = await GetCommentLikers(_client, newCommentId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(likersBefore.Count > likersAfter.Count);
        }

        [Theory(DisplayName = "PUT CommentLikeStatus InvalidCall comment not exists")]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Put_CommentLikeStatus_InvalidCall_NotExists(bool like)
        {
            int userId = await GetFirstUserId(_client);

            var response = await PutCommentLikeStatus(_client, like, -1, userId);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory(DisplayName = "PUT CommentLikeStatus InvalidCall user not exists")]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Put_CommentLikeStatus_InvalidCall_UserNotExists(bool like)
        {
            int commentId = await GetFirstCommentId(_client);

            var response = await PutCommentLikeStatus(_client, like, commentId, -1);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory(DisplayName = "PUT CommentLikeStatus ValidCall 2x same status")]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Put_CommentLikeSameStatus_ValidCall(bool like)
        {
            int userId = await GetFirstUserId(_client);

            var newCommentResponse = await PostNewComment(_client, userId);
            var stringResponse = await newCommentResponse.Content.ReadAsStringAsync();
            int newCommentId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;
            var response = await PutCommentLikeStatus(_client, like, newCommentId, userId);

            var likersBefore = await GetCommentLikers(_client, newCommentId);

            response = await PutCommentLikeStatus(_client, like, newCommentId, userId);

            var likersAfter = await GetCommentLikers(_client, newCommentId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(likersBefore.Count == likersAfter.Count);
        }
    }
}
