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

}