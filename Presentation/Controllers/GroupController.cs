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
        [Authorize]
        public async Task<ActionResult<GroupCreateResult>> Create([FromBody] GroupCreateDto request)
        {
            var result = new GroupCreateResult();
            string UserIdString = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                result.ErrorCode = 401;
                result.Message = "Unauthorized";
                return result;
            }

            var command = new GroupCreateCommand()
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = UserIdString,
            };

            return await mediator.Send(command);
        }

        [HttpGet(nameof(Search))]
        [Authorize]
        public async Task<ActionResult<GroupSearchResult>> Search(string name)
        {
            var result = new GroupSearchResult();
            string UserIdString = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                result.ErrorCode = 401;
                result.Message = "Unauthorized";
                return result;
            }

            var request = new GroupSearchCommand()
            {
                Name = name,
                SearcherId = UserIdString
            };

            return await mediator.Send(request);
        }

    }
}
