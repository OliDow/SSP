namespace SSP.Common.Messaging.Messaging;

public class MessageContext : IMessageContext
{
    public string CorrelationId { get; set; } = "Not Set";
    public string? MessageId { get; set; }
    public string? RequestOrigin { get; set; }
}

public interface IMessageContext
{
    string CorrelationId { get; set; }
    string? MessageId { get; set; }
    string? RequestOrigin { get; set; }
}