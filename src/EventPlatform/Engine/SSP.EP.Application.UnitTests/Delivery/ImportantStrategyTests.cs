using Moq;
using SSP.Common.Messaging.Messaging;
using SSP.EP.Application.Delivery;
using SSP.Events.Enums;

namespace SSP.EP.Application.UnitTests.Delivery;

public class ImportantStrategyTests
{
    private readonly Mock<IBusClient> _mockBusClient = new(MockBehavior.Strict);
    private readonly IDeliveryStrategy _sut;


    public ImportantStrategyTests()
    {
        _sut = new ImportantStrategy(_mockBusClient.Object);
    }

    [Fact]
    public async Task WhenPublishing_ShouldCallBusClientForEachMessage()
    {
        // arrange
        DeliveryEvent event1 = new("1", new List<EventContext> { EventContext.Digital, EventContext.Auditing });
        DeliveryEvent event2 = new("2", new List<EventContext> { EventContext.Umax, EventContext.Auditing });
        List<DeliveryEvent> dataEvent = new() { event1, event2 };

        _mockBusClient.Setup(s => s.PublishToTopicAsync(event1.Payload,
                $"{event1.Context.First()}|{event1.Context.Last()}", null, CancellationToken.None))
            .ReturnsAsync(It.IsAny<string>());
        _mockBusClient.Setup(s => s.PublishToTopicAsync(event2.Payload,
                $"{event2.Context.First()}|{event2.Context.Last()}", null, CancellationToken.None))
            .ReturnsAsync(It.IsAny<string>());

        // act
        // assert
        await _sut.PublishAsync(dataEvent);
    }
}