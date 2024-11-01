using Application.Common.Models;
using Application.Event;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    
}