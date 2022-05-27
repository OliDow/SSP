using SSP.Common.Extensions;

// ReSharper disable InvokeAsExtensionMethod

namespace SSP.Common.Messaging.Provider;

public class EventProvider<T> : IEventProvider<T>
    where T : IEvent
{
    private readonly List<Type> _events;

    public EventProvider()
    {
        _events = AssemblyExtensions.FindDerivedTypes(typeof(T).Assembly, typeof(IEvent));
    }

    public Type GetEventType(string type)
    {
        try
        {
            return _events.Single(t => t.Name == type);
        }
        catch (Exception e)
        {
            throw new Exception("Event Type not found in Type List", e);
        }
    }
}