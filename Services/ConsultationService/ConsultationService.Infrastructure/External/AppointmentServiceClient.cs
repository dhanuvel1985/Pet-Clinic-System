using ConsultationService.Application.DTOs;
using ConsultationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Infrastructure.External
{
    public class AppointmentServiceClient : IAppointmentServiceClient
    {
        private readonly HttpClient _http;

        public AppointmentServiceClient(HttpClient http, IConfiguration config)
        {
            http.BaseAddress = new Uri(config["Services:AppointmentServiceUrl"]);
            _http = http;
        }

        public async Task<AppointmentDto?> GetAppointment(Guid appointmentId)
        {
            var response = await _http.GetAsync($"/api/v1/appointments/{appointmentId}");

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<AppointmentDto>();
        }
    }
}
