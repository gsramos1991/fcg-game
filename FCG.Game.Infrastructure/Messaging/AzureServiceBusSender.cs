using Azure.Messaging.ServiceBus;
using FCG.Game.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Game.Infrastructure.Messaging
{
    public class AzureServiceBusConfig 
    {
        public string ConnectionString { get; set; }
    }
    public class AzureServiceBusSender : IMessagePublisher
    {
        private readonly AzureServiceBusConfig _config;
        
        public AzureServiceBusSender(IOptions<AzureServiceBusConfig> config)
        {
            _config = config.Value;
        }
        public async Task Publish(string message, string queueName)
        {
            try
            {
                await using var cliente = new ServiceBusClient(_config.ConnectionString);
                await using ServiceBusSender sender = cliente.CreateSender(queueName);
                var msgSender = new ServiceBusMessage(message);

                await sender.SendMessageAsync(msgSender);
            }
            catch (Exception ex)
            {
                
                throw;
            }

        }

        
    }
}
