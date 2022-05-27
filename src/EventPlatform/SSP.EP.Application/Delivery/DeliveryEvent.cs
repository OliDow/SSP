namespace SSP.EP.Application.Delivery;

public record DeliveryEvent(object Payload, List<string> Context);