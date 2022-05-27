using System.Collections.ObjectModel;

namespace SSP.EP.Application.Repositories;

public interface IEventConfigRepository
{
    Task<EventConfig> GetAsync(string eventName);
    Task<Collection<EventConfig>> GetAsync(List<string> eventNames);
}