using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using SSP.Common.Messaging;
using SSP.Common.Messaging.Functions;
using SSP.Common.Messaging.Functions.Builders;
using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Provider;
using SSP.Common.Messaging.Repository;
using SSP.EP.Application.Delivery;
using SSP.EP.Application.Repositories;
using SSP.Events;
using System.Text.Json;

namespace SSP.EP.FunctionApp;

public class EventRouter : ServiceBusTriggerBase
{
    private readonly IEventProvider<CreateAccount> _eventProvider;
    private readonly IEventConfigRepository _eventConfigRepository;
    private readonly IList<IDeliveryStrategy> _deliveryStrategies;

    public EventRouter(IMessageContext messageContext, IErrorMetadataBuilder errorMetadataBuilder,
        IEventProvider<CreateAccount> eventProvider, // need to wrap this to prevent the need to specify a type when injecting
        IEventSchemaRepository eventSchemaRepository, IEventConfigRepository eventConfigRepository,
        IList<IDeliveryStrategy> deliveryStrategies)
        : base(messageContext, errorMetadataBuilder, eventSchemaRepository)
    {
        _eventConfigRepository = eventConfigRepository ?? throw new ArgumentNullException(nameof(eventConfigRepository));
        _deliveryStrategies = deliveryStrategies ?? throw new ArgumentNullException(nameof(deliveryStrategies));
        _eventProvider = eventProvider ?? throw new ArgumentNullException(nameof(eventProvider));
    }

    [FunctionName("EventRouter")]
    public async Task RunAsync(
        [EventHubTrigger("ingest", ConsumerGroup = "$Default", Connection = "Connection")]
        EventData[] events, // Can we accept CloudEvents natively? Need to find out Attributes work for correlationId
        CancellationToken cancellationToken)
    {
        // todo Bulk send? Would not be atomic
        foreach (var @event in events)
        {
            // Try catch to make each item atomic, could be completed in parallel
            try
            {
                // deserialise each message
                var messageType = @event.Properties["MessageType"].ToString() ?? string.Empty;
                // @event..TryGetValue("MessageType", out var messageType);

                var type = _eventProvider.GetEventType(messageType.ToString());

                var innerEvent = ParseEvent(@event.BodyAsStream, type);
                // IEvent innerEvent = JsonSerializer.Deserialize(@event.BodyAsStream, type);

                // Check Schema registry
                await EventSchemaRepository.DoEventRepositoryStuff();

                // look up message type config, check for transport mechanism
                // todo Fetch in bulk _eventConfigRepository.GetAsync(List<>)
                var eventConfig = await _eventConfigRepository.GetAsync(type.Name); // Could be gathered

                // Deliver to each destination
                foreach (var destination in eventConfig.Destinations)
                {
                    var strategy = _deliveryStrategies.Single(s => s.StrategyName == destination.Strategy);
                    await strategy.PublishAsync(new List<DeliveryEvent> { new(innerEvent, destination.Context) });
                }
            }
            catch (Exception e)
            {
                // Atomic transaction failed, log this
                Console.WriteLine(e.ToString());
            }
        }
    }

    private static IEvent ParseEvent(Stream body, Type type)
    {
        if (JsonSerializer.Deserialize(body, type) is IEvent innerEvent)
        {
            return innerEvent;
        }

        throw new ArgumentOutOfRangeException($"Event failed to parse to {type.Name}");
    }
}