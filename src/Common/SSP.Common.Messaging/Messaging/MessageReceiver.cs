using Azure.Messaging.ServiceBus;
using MediatR;
using System.Text.Json;

namespace SSP.Common.Messaging.Messaging;

internal class MessageReceiver : MessageReceiverBase<ServiceBusReceivedMessage>
{
    public MessageReceiver(IMediator mediator)
        : base(mediator) { }

    protected override IMessage GetMessage(ServiceBusReceivedMessage busMessage)
    {
        var messageTypeProp = busMessage.ApplicationProperties.GetValueOrDefault(Constants.MessageTypePropertyName);
        var messageType = Type.GetType(messageTypeProp?.ToString() ?? string.Empty, true);
        var payloadJson = busMessage.Body.ToString();
        var payload = JsonSerializer.Deserialize(payloadJson, messageType!) as IMessage;
        return payload!;
    }
}