using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Repository;

// ReSharper disable InvokeAsExtensionMethod
namespace SSP.Common.Messaging.Functions;

public abstract class TriggerBase
{
    protected IMessageContext MessageContext { get; }
    protected IEventSchemaRepository EventSchemaRepository { get; }

    protected TriggerBase(IMessageContext messageContext, IEventSchemaRepository eventSchemaRepository)
    {
        MessageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
        EventSchemaRepository = eventSchemaRepository ?? throw new ArgumentNullException(nameof(eventSchemaRepository));
    }
}