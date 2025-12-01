using AppointmentService.Application.Events;
using AppointmentService.Application.Interfaces;
using AppointmentService.Infrastructure.Notifications;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace AppointmentService.Api.BackgroundServices
{
    public class AppointmentCreatedConsumer : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly INotificationService _notify;

        public AppointmentCreatedConsumer(IConfiguration config, INotificationService notify)
        {
            var client = new ServiceBusClient(config["AzureServiceBus:ConnectionString"]);

            _processor = client.CreateProcessor(
                config["AzureServiceBus:AppointmentCreatedTopic"],
                subscriptionName: "receptionist-sub");

            _notify = notify;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += async args =>
            {
                var data = JsonSerializer.Deserialize<AppointmentCreatedEvent>(args.Message.Body);
                await _notify.NotifyReceptionistAsync(data);
            };

            _processor.ProcessErrorAsync += errorArgs => Task.CompletedTask;

            await _processor.StartProcessingAsync(stoppingToken);
        }
    }

}
