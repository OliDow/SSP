using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Repository;

// ReSharper disable InvokeAsExtensionMethod
namespace SSP.Common.Messaging.Functions;

public class TriggerBase
{
    protected readonly IMessageContext MessageContext;
    protected readonly IEventSchemaRepository EventSchemaRepository;

    protected TriggerBase(IMessageContext messageContext, IEventSchemaRepository eventSchemaRepository)
    {
        MessageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
        EventSchemaRepository = eventSchemaRepository ?? throw new ArgumentNullException(nameof(eventSchemaRepository));
    }
}