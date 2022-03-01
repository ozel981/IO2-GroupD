using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SaleSystem;
using SaleSystem.Models.Newsletters;
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
    public class NewsletterIntegrationTests : AbstractIntegrationTests
    {
        [Fact(DisplayName = "POST Subscription ValidCall")]
        public async Task Post_Subscribe_ValidCall()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var subInfo = new SubscriptionInfo
            {
                CategoryID = await GetFirstCategorieId(_client),
                IsSubscribed = true,
            };
            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var subInfoJson = JsonConvert.SerializeObject(subInfo);

            HttpContent content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "POST Subscription InvalidCall subscription already exists")]
        public async Task Post_Subscribe_InvalidCall_SubscriptionAlreadyExists()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var subInfo = new SubscriptionInfo
            {
                CategoryID = await GetFirstCategorieId(_client),
                IsSubscribed = true,
            };
            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var subInfoJson = JsonConvert.SerializeObject(subInfo);

            HttpContent content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST Subscription ValidCall unsubscribe")]
        public async Task Post_Unsubscribe_ValidCall()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var subInfo = new SubscriptionInfo
            {
                CategoryID = await GetFirstCategorieId(_client),
                IsSubscribed = true,
            };
            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());
            var subInfoJson = JsonConvert.SerializeObject(subInfo);

            HttpContent content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            subInfo.IsSubscribed = false;
            subInfoJson = JsonConvert.SerializeObject(subInfo);

            content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");
            response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "POST Subscription InvalidCall subscription not exists")]
        public async Task Post_Subscription_InvalidCall_SubscriptionNotExists()
        {
            var newUserResponse = await PostNewUser(_client);
            var stringResponse = await newUserResponse.Content.ReadAsStringAsync();
            var newUserId = JsonConvert.DeserializeObject<ResponseWithId>(stringResponse).Id;

            var subInfo = new SubscriptionInfo
            {
                CategoryID = await GetFirstCategorieId(_client),
                IsSubscribed = false,
            };

            _client.DefaultRequestHeaders.Add("userID", newUserId.ToString());

            var subInfoJson = JsonConvert.SerializeObject(subInfo);
            HttpContent content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST Subscription InvalidCall user not exists")]
        public async Task Post_Subscription_InvalidCall_UserNotExists()
        {
            var subInfo = new SubscriptionInfo
            {
                CategoryID = await GetFirstCategorieId(_client),
                IsSubscribed = false,
            };

            _client.DefaultRequestHeaders.Add("userID", "-1");

            var subInfoJson = JsonConvert.SerializeObject(subInfo);
            HttpContent content = new StringContent(subInfoJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(client_url + "/newsletter", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
