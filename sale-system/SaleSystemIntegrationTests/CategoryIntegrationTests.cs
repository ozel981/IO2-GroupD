using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SaleSystem;
using SaleSystem.Models.Categories;
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
    public class CategoryIntegrationTests : AbstractIntegrationTests
    {

        [Fact(DisplayName = "GET Categories ValidCall")]
        public async Task Get_AllCategories()
        {
            var response = await _client.GetAsync(client_url + "/categories");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryView>>(stringResponse);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(categories);
        }
    }
}
