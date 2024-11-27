using Application.Common.Models;
using MediatR;

namespace Application.Events;

public class SaveEventCommand : IRequest<SaveEventResult>
{
}

public class SaveEventResult : ResultModel
{
}

public class SaveEventHandler : IRequestHandler<SaveEventCommand, SaveEventResult>
{
    public async Task<SaveEventResult> Handle(SaveEventCommand request, CancellationToken cancellationToken)
    {
        var result = new SaveEventResult();
        result.Success = true;
        return result;
    }
}