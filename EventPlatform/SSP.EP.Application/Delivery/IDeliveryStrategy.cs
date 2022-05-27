using SSP.Events.Enums;

namespace SSP.EP.Application.Delivery;

public interface IDeliveryStrategy
{
    public EventDestination StrategyName { get; set; }
    Task PublishAsync(List<DeliveryEvent> dataEvents);
}