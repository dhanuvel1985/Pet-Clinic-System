using AppointmentService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.External
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserServiceClient(HttpClient http, IConfiguration config, IHttpContextAccessor accessor)
        {
            //http.BaseAddress = new Uri(config["Services:UserServiceUrl"]);
            _http = http;
            _httpContextAccessor = accessor;
        }

        public async Task<bool> UserExists(Guid userId)
        {
            // Get incoming Authorization header
            var token = _httpContextAccessor
                            .HttpContext?
                            .Request
                            .Headers["Authorization"]
                            .ToString();
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/auth/{userId}");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Add("Authorization", token);

            var response = await _http.SendAsync(request);
            //var response = await _http.GetAsync($"api/v1/auth/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
