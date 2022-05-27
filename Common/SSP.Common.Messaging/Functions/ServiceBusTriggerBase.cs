using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs.ServiceBus;
using SSP.Common.Messaging.Messaging;
using System.Text.Json;

namespace SSP.Common.Messaging.Functions;

public abstract class ServiceBusTriggerBase
{
    private readonly IMessageContext _messageContext;
    private readonly IServiceBusTriggerServiceProvider _serviceProvider;

    protected ServiceBusTriggerBase(IMessageContext messageContext, IServiceBusTriggerServiceProvider serviceProvider)
    {
        _messageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    protected async Task ProcessMessageAsync(
        ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, CancellationToken cancellationToken)
    {
        try
        {
            // Abstracted execution to the provider? This can be shared then with the implementation in the funcs application
            // await _serviceProvider.MessageReceiver.ReceiveAsync(message, cancellationToken);

            // Abstract this to shared code
            var messageTypeProp = message.ApplicationProperties.GetValueOrDefault(Constants.MessageTypePropertyName);
            var messageType = Type.GetType(messageTypeProp?.ToString() ?? string.Empty, true);
            var payloadJson = message.Body.ToString();
            var payload = JsonSerializer.Deserialize(payloadJson, messageType!) as IMessage;
            // await _mediator.Send(payload, cancellationToken)

            Console.WriteLine("PrepareViewGenerator");
            Console.WriteLine(payload);

            // Here to demo message exception logging
            if (messageType.Name == "CreateAccount")
            {
                throw new ArgumentException("I have a bad feeling about this!");
            }

            await messageActions.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Override the AbandonMessage to enrich message with exception data
            var propertiesToModify = _serviceProvider.ErrorMetadataBuilder.BuildErrorMetadata(message.DeliveryCount, ex);
            await messageActions.AbandonMessageAsync(message, propertiesToModify, cancellationToken);
            throw;
        }
    }
}