using SSP.Common;

namespace SSP.Events;

// set all these as string to not make any assumptions
public record SubmitMeterReading(string MeterPointId, string Reading) : IEvent
{
    // playground

    // var correlationIdAttribute = CloudEventAttribute.CreateExtension("CorrelationId", CloudEventAttributeType.String);
    //
    // CloudEvent
    //     cloudEvent.ExtensionAttributes.add
    //
    // var cloudEvent = new CloudEvent(new List<CloudEventAttribute>
    // {
    //     correlationIdAttribute
    // })
    // {
    //     Type = "CheeseType",
    //     Data = new Cheese("Edam"),
    //     ExtensionAttributes = new CloudEventAttribute[]
    // };
    //
    // cloudEvent.AddServiceBusClient()
    // cloudEvent.to
    // var t = new CloudNative.CloudEvents.AvroEventFormatter<string>();
}