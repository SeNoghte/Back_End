using Application.Groups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    public class GroupController : BaseController
    {
        public GroupController(IMediator mediator) : base(mediator)
        {

        }

        [HttpPost(nameof(Create))]
        public async Task<ActionResult<GroupCreateResult>> Create([FromBody] GroupCreateCommand request)
        {
            return await mediator.Send(request);
        }

        [HttpPost(nameof(GetGroups))]
        public async Task<ActionResult<GroupSearchResult>> GetGroups([FromBody] GroupSearchCommand request)
        {
            return await mediator.Send(request);
        }

        [HttpPost(nameof(Delete))]
        public async Task<ActionResult<GroupDeleteResult>> Delete([FromBody] GroupDeleteCommand request)
        {
            return await mediator.Send(request);
        }    
    }
}
