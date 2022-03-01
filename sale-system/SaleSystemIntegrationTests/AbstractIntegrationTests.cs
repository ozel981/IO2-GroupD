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
    class ResponseWithId
    {
        public int Id { get; set; }
    }
    public partial class AbstractIntegrationTests : IDisposable
    {
        //Set this variable to run test on another client
        protected readonly bool isAnotherClient = false;
        protected readonly string client_url = "";
        //protected readonly string client_url = "https://serverappts.azurewebsites.net/";
        //protected readonly string client_url = "https://systempromocji.azurewebsites.net/";

        protected readonly HttpClient _client;
        protected readonly SaleSystemWebApplicationFactory<Startup> _factory;
        public AbstractIntegrationTests()
        {
            if (isAnotherClient)
            {
                _client = new HttpClient();
            }
            else
            {
                _factory = new SaleSystemWebApplicationFactory<Startup>();
                _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            }
        }

        protected async Task<bool> IsEmailAlreadyExist(HttpClient client, string email)
        {
            var response = await _client.GetAsync(client_url + "/users");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserView>>(stringResponse);

            return users.Any(u => u.UserEmail == email);
        }

        protected async Task<string> GetUnoccupiedEmail(HttpClient client, string emailPrefix = "newuser", string emailSufix = "@mailbox.com")
        {
            int emailNr = 0;
            string email;
            do
            {
                email = $"{emailPrefix}{emailNr.ToString()}{emailSufix}";
                emailNr++;
            } while (await IsEmailAlreadyExist(client, email.ToString()));
            return email;
        }

        public void Dispose()
        {
            _client.Dispose();
            if (!isAnotherClient)
                _factory.Dispose();
        }
    }
}
