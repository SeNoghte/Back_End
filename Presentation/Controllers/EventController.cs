using Application.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class EventController : BaseController
{
    public EventController(IMediator mediator) : base(mediator)
    {

    }

    [HttpPost(nameof(GetMyEvents))]
    public async Task<ActionResult<GetMyEventsResult>> GetMyEvents([FromBody] GetMyEventsQuery request)
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

    [HttpPost(nameof(GetPublicTasksListByTag))]
    public async Task<ActionResult<GetPublicEventListByTagResult>> GetPublicTasksListByTag([FromBody] GetPublicEventsListByTagQuery request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(GetPublicEventListSearch))]
    public async Task<ActionResult<GetPublicEventListSearchResult>> GetPublicEventListSearch([FromBody] GetPublicEventListSearchQuery request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(GetMostUsedTagsList))]
    public async Task<ActionResult<GetMostUsedTagsListResult>> GetMostUsedTagsList([FromBody] GetMostUsedTagsListQuery request)
    {
        return await mediator.Send(request);
    }

    [HttpPost(nameof(LeaveEvent))]
    public async Task<ActionResult<LeaveEventResult>> LeaveEvent([FromBody] LeaveEventCommand request)
    {
        return await mediator.Send(request);
    }

}