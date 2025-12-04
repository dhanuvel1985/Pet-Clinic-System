using ConsultationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Infrastructure.External
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _http;

        public UserServiceClient(HttpClient http, IConfiguration config)
        {
            http.BaseAddress = new Uri(config["Services:UserServiceUrl"]);
            _http = http;
        }

        public async Task<bool> UserExists(Guid userId)
        {
            var response = await _http.GetAsync($"/api/v1/users/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
