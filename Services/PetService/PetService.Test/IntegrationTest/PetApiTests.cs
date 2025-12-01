using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Test.IntegrationTest
{
    public class PetApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PetApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreatePet_Returns_Created()
        {
            var request = new { name = "TestPet", species = "Cat", age = 3, breed = "Siamese", ownerId = Guid.NewGuid() };

            var response = await _client.PostAsJsonAsync("/api/v1/pets", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
