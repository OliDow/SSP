namespace SSP.Common.Messaging.Messaging;

public interface IMessageContext
{
    string CorrelationId { get; set; }
    string? MessageId { get; set; }
    string? RequestOrigin { get; set; }
}