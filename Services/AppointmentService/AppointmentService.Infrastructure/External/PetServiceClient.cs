using AppointmentService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.External
{
    public class PetServiceClient : IPetServiceClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PetServiceClient(HttpClient http, IConfiguration config, IHttpContextAccessor accessor)
        {
            //http.BaseAddress = new Uri(config["Services:PetServiceUrl"]);
            _http = http;
            _httpContextAccessor = accessor;
        }

        public async Task<bool> PetExists(Guid petId)
        {
            // Get incoming Authorization header
            var token = _httpContextAccessor
                            .HttpContext?
                            .Request
                            .Headers["Authorization"]
                            .ToString();

            Console.WriteLine("Incoming Authorization: " + token);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/pets/{petId}");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Add("Authorization", token);

            var response = await _http.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;

            //var response = await _http.SendAsync(request);

            ////var response = await _http.GetAsync($"api/v1/pets/{petId}");
            //return response.IsSuccessStatusCode;
        }
    }

}
