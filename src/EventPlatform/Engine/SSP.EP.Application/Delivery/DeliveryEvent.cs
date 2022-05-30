using SSP.Events.Enums;

namespace SSP.EP.Application.Delivery;

public record DeliveryEvent(object Payload, List<EventContext> Context);