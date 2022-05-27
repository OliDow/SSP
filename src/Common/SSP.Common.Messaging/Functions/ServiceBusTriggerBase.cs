using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Azure.WebJobs.ServiceBus;
using SSP.Common.Messaging.Functions.Builders;
using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Repository;
using System.Text.Json;

namespace SSP.Common.Messaging.Functions;

public abstract class ServiceBusTriggerBase : TriggerBase
{
    private readonly IErrorMetadataBuilder _errorMetadataBuilder;

    protected ServiceBusTriggerBase(
        IMessageContext messageContext,
        IErrorMetadataBuilder errorMetadataBuilder,
        IEventSchemaRepository eventSchemaRepository)
        : base(messageContext, eventSchemaRepository)
    {
        _errorMetadataBuilder = errorMetadataBuilder ?? throw new ArgumentNullException(nameof(errorMetadataBuilder));
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

            Console.WriteLine("PrepareViewGenerator");
            Console.WriteLine(payload);

            // Here to demo message exception logging
            if (messageType!.Name == "CreateAccount")
            {
                throw new ArgumentException("I have a bad feeling about this!");
            }

            await messageActions.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Override the AbandonMessage to enrich message with exception data
            var propertiesToModify = _errorMetadataBuilder.BuildErrorMetadata(message.DeliveryCount, ex);
            await messageActions.AbandonMessageAsync(message, propertiesToModify, cancellationToken);
            throw;
        }
    }
}