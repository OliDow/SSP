using MediatR;

namespace SSP.Common.Messaging.Messaging;

public abstract class MessageReceiverBase<TBusMessage> : IMessageReceiver<TBusMessage>
{
    private readonly IMediator _mediator;

    protected MessageReceiverBase(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task ReceiveAsync(TBusMessage busMessage, CancellationToken cancellationToken)
    {
        var message = GetMessage(busMessage);
        return _mediator.Send(message!, cancellationToken);
    }

    protected abstract IMessage GetMessage(TBusMessage busMessage);
}