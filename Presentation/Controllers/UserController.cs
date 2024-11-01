using Application.Users;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IMediator mediator) : base(mediator)
        {
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginCommand requset)
        {
            return await mediator.Send(requset);
        }

        [AllowAnonymous]
        [HttpPost(nameof(SignUp))]
        public async Task<ActionResult<SignUpResult>> SignUp([FromBody] SignUpCommand requset)
        {
            return await mediator.Send(requset);
        }

        [AllowAnonymous]
        [HttpPost(nameof(GoogleLogin))]
        public async Task<ActionResult<GoogleLoginResult>> GoogleLogin([FromBody] GoogleLoginCommand request)
        {
            return await mediator.Send(request);
        }
    }
}
