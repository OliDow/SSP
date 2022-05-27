using Microsoft.Extensions.Configuration;
using SSP.Common.Messaging.Messaging;

namespace SSP.Common.Messaging.EventHub;

public class EventEventHubClient : IEventHubClient
{
    public EventEventHubClient(IMessageContext messageContext, IConfiguration configuration) { }
}

public interface IEventHubClient { }