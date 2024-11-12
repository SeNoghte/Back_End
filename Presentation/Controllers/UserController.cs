using Application.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [AllowAnonymous]
        [HttpPost(nameof(SendVerificationCode))]
        public async Task<ActionResult<SendVerificationCodeResult>> SendVerificationCode([FromBody] SendVerificationCodeCommand request)
        {
            return await mediator.Send(request);
        }

        [AllowAnonymous]
        [HttpPost(nameof(VerifyVerificationCode))]
        public async Task<ActionResult<VerifyCodeResult>> VerifyVerificationCode([FromBody] VerifyCodeCommand request)
        {
            return await mediator.Send(request);
        }
    }
}
