namespace SSP.Common.Messaging.Messaging;

public class MessageContext : IMessageContext
{
    public string CorrelationId { get; set; } = "Not Set";
    public string? MessageId { get; set; }
    public string? RequestOrigin { get; set; }
}