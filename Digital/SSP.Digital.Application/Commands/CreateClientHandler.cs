using MediatR;

namespace SSP.Digital.Application.Commands;

public class CreateClientHandler : IRequestHandler<CreateClient>
{
    public Task<Unit> Handle(CreateClient request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}