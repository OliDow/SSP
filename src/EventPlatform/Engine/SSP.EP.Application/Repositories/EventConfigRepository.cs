using SSP.Events.Enums;
using System.Collections.ObjectModel;

namespace SSP.EP.Application.Repositories;

public class EventConfigRepository : IEventConfigRepository
{
    public async Task<EventConfig> GetAsync(string eventName)
    {
        await Task.CompletedTask;

        return eventName switch
        {
            "CreateAccount" => new EventConfig(eventName,
                new List<EventConfig.Destination>
                {
                    new(EventDestination.Important, new List<EventContext> { EventContext.Digital, EventContext.Auditing })
                }),
            "SubmitMeterReading" => new EventConfig(eventName,
                new List<EventConfig.Destination>
                {
                    new(EventDestination.Important, new List<EventContext> { EventContext.Umax, EventContext.BI })
                }),
            _ => throw new ArgumentOutOfRangeException(eventName)
        };
    }

    public Task<Collection<EventConfig>> GetAsync(List<string> eventNames)
    {
        throw new NotImplementedException();
    }
}

// Move from here