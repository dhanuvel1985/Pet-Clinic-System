using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Infrastructure.EventBus
{
    public class ServiceBusPublisher
    {
        private readonly ServiceBusClient _client;

        public ServiceBusPublisher(ServiceBusClient client)
        {
            _client = client;
        }

        public async Task PublishAsync(string topic, object message)
        {
            var sender = _client.CreateSender(topic);
            var body = System.Text.Json.JsonSerializer.Serialize(message);

            await sender.SendMessageAsync(new ServiceBusMessage(body));
        }
    }
}
