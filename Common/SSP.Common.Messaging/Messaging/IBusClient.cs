namespace SSP.Common.Messaging.Messaging;

public interface IBusClient
{
    Task<string> PublishToTopicAsync<TM>(TM message, string subject, string? sessionId, CancellationToken cancellationToken)
        where TM : class;
}