using AppointmentService.Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.External
{
    public class ServiceBusMessagePublisher : IMessageBusPublisher
    {
        private readonly ServiceBusClient _client;

        public ServiceBusMessagePublisher(IConfiguration config)
        {
            _client = new ServiceBusClient(config["AzureServiceBus:ConnectionString"]);
        }

        public async Task PublishAsync(string topic, object message)
        {
            var sender = _client.CreateSender(topic);

            string json = JsonSerializer.Serialize(message);
            await sender.SendMessageAsync(new ServiceBusMessage(json));
        }
    }
}
