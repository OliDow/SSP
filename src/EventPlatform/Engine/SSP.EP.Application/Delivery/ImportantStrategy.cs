using SSP.Common.Messaging.Messaging;
using SSP.Events.Enums;

namespace SSP.EP.Application.Delivery;

public class ImportantStrategy : IDeliveryStrategy
{
    public EventDestination StrategyName { get; set; } = EventDestination.Important;
    private readonly IBusClient _busClient;

    public ImportantStrategy(IBusClient busClient)
    {
        _busClient = busClient ?? throw new ArgumentNullException(nameof(busClient));
    }

    public async Task PublishAsync(List<DeliveryEvent> dataEvents)
    {
        foreach (var dataEvent in dataEvents)
        {
            var subject = string.Join('|', dataEvent.Context);
            await _busClient.PublishToTopicAsync(dataEvent.Payload, subject, null, CancellationToken.None);
        }
    }
}