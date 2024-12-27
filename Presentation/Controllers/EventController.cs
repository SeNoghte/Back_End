using Application.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class EventController : BaseController
{
    public EventController(IMediator mediator) : base(mediator)
    {

    }

    [HttpPost(nameof(SaveEvent))]
    public async Task<ActionResult<SaveEventResult>> SaveEvent([FromBody] SaveEventCommand request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(Create))]
    public async Task<ActionResult<CreateEventResult>> Create([FromBody] CreateEventCommand request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(GetEvent))]
    public async Task<ActionResult<GetEventResult>> GetEvent([FromBody] GetEventQuery request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(JoinEvent))]
    public async Task<ActionResult<JoinEvenResult>> JoinEvent([FromBody] JoinEventCommand request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(AssignTaskToMe))]
    public async Task<ActionResult<AssignTaskToUserResult>> AssignTaskToMe([FromBody] AssignTaskToUserCommand request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(RemoveTaskFromMe))]
    public async Task<ActionResult<RemoveTaskFromUserResult>> RemoveTaskFromMe([FromBody] RemoveTaskFromUserCommand request)
    {
        return await mediator.Send(request);
    }

}