using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using SSP.Common.Providers;
using System.Text;
using System.Text.Json;

namespace SSP.Common.Messaging.Messaging;

public class BusClient : IBusClient
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IMessageContext _messageContext;
    private readonly IGuidProvider _guidProvider;

    public BusClient(ServiceBusClient serviceBusClient, IMessageContext messageContext, IConfiguration configuration,
        IGuidProvider guidProvider)
    {
        _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
        _messageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
        _publishTopicName = configuration["OutEventTopicName"];
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
    }

    private readonly string _publishTopicName;

    public async Task<string> PublishToTopicAsync<TM>(TM message, string subject, string? sessionId,
        CancellationToken cancellationToken)
        where TM : class
    {
        var serviceBusSender = _serviceBusClient.CreateSender(_publishTopicName);
        var messageId = _guidProvider.NewGuid().ToString();

        var jsonEvent = JsonSerializer.Serialize(message);

        var busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonEvent))
        {
            CorrelationId = _messageContext.CorrelationId,
            MessageId = messageId,
            Subject = subject,
            SessionId = sessionId
        };

        busMessage.ApplicationProperties.Add(Constants.MessageTypePropertyName, message.GetType().AssemblyQualifiedName);

        // todo Need to plan where these sit on the event wrapper
        // busMessage.ApplicationProperties.Add(nameof(MessageMetadata.MessageOriginator), metadata.MessageOriginator);
        // busMessage.ApplicationProperties.Add(nameof(MessageMetadata.RequestOriginator), metadata.RequestOriginator);
        // busMessage.ApplicationProperties.Add(nameof(MessageMetadata.SentUtcDateTime), metadata.SentUtcDateTime);

        await serviceBusSender.SendMessageAsync(busMessage, cancellationToken);
        return messageId;
    }
}