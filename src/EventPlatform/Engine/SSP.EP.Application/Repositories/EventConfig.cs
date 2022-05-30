using SSP.Events.Enums;

namespace SSP.EP.Application.Repositories;

public record EventConfig(string Name, List<EventConfig.Destination> Destinations)
{
    public record Destination(EventDestination Strategy, List<EventContext> Context);
}