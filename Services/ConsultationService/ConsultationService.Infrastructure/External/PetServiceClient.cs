using ConsultationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Infrastructure.External
{
    public class PetServiceClient : IPetServiceClient
    {
        private readonly HttpClient _http;

        public PetServiceClient(HttpClient http, IConfiguration config)
        {
            http.BaseAddress = new Uri(config["Services:PetServiceUrl"]);
            _http = http;
        }

        public async Task<bool> PetExists(Guid petId)
        {
            var response = await _http.GetAsync($"/api/v1/pets/{petId}");
            return response.IsSuccessStatusCode;
        }
    }

}
