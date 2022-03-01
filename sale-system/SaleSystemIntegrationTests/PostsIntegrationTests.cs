using Newtonsoft.Json;
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
    public class PostsIntegrationTests : AbstractIntegrationTests
    {
        [Fact(DisplayName = "GET Posts ValidCall")]
        public async Task Get_Posts_ValidCall()
        {
            var newPostResponse = await PostNewPost(_client);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            int userId = await GetFirstUserId(_client);
            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/posts");

            stringResponse = await response.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<PostView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(posts);
            Assert.True(posts.Exists(post => post.ID == newPostId));
        }

        [Fact(DisplayName = "GET Posts InvalidCall user nor exists")]
        public async Task Get_Posts_InvalidCall_UserNotExists()
        {
            var newPostResponse = await PostNewPost(_client);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var response = await _client.GetAsync(client_url + "/posts");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "GET UserPosts ValidCall")]
        public async Task Get_UserPosts_ValidCall()
        {
            int userId = await GetFirstUserId(_client);
            var newPostResponse = await PostNewPost(_client, userId);
            var stringResponse = await newPostResponse.Content.ReadAsStringAsync();
            int newPostId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", userId.ToString());

            var response = await _client.GetAsync(client_url + "/posts/" + userId);

            stringResponse = await response.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<PostView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(posts);
            Assert.True(posts.Exists(post => post.ID == newPostId));
        }

        [Fact(DisplayName = "GET UserPosts ValidCall empty")]
        public async Task Get_UserPosts_ValidCall_NoPosts()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());

            var response = await _client.GetAsync(client_url + "/posts/" + newUserId);

            stringResponse = await response.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<PostView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(posts);
        }

        [Fact(DisplayName = "GET UserPosts InvalidCall user not exists")]
        public async Task Get_UserPosts_InvalidCall_UserNotExists()
        {
            var response = await _client.GetAsync(client_url + "/posts/-1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
