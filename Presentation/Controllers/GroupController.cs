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

        [HttpPost(nameof(Search))]
        public async Task<ActionResult<GroupSearchResult>> Search([FromBody] GroupSearchCommand request)
        {
            return await mediator.Send(request);
        }

    }
}
