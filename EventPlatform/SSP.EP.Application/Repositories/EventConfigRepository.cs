using SSP.Events.Enums;
using System.Collections.ObjectModel;

namespace SSP.EP.Application.Repositories;

public class EventConfigRepository : IEventConfigRepository
{
    public async Task<EventConfig> GetAsync(string eventName) =>
        eventName switch
        {
            "CreateAccount" => new EventConfig(eventName,
                new List<EventConfig.Destination>
                {
                    new(EventDestination.Important, new List<string> { "meter", "account" })
                }),
            "SubmitMeterReading" => new EventConfig(eventName,
                new List<EventConfig.Destination>
                {
                    new(EventDestination.Important, new List<string> { "meter" })
                }),
            _ => throw new ArgumentOutOfRangeException(eventName)
        };

    public Task<Collection<EventConfig>> GetAsync(List<string> eventNames)
    {
        throw new NotImplementedException();
    }
}

// Move from here
public record EventConfig(string Name, List<EventConfig.Destination> Destinations)
{
    public record Destination(EventDestination Strategy, List<string> Context);
}