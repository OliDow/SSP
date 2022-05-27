using MediatR;

namespace SSP.Digital.ProjGen.Application.Commands;

public class CreateClientHandler : IRequestHandler<CreateClient>
{
    public Task<Unit> Handle(CreateClient request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}