using Application.Common.Models;
using Application.Event;
using Application.User;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

public class UserController : BaseController
{
    public UserController(IMediator mediator) : base(mediator)
    {
        
    }

    [HttpGet("signin-google")] 
    public async Task<ActionResult<SignUpResult>> GoogleSignIn()
    {
       
        var properties = new AuthenticationProperties { RedirectUri = "notconfigured" }; //  should put redirect address
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        var x = HttpContext;
        
        return Ok(new SignUpResult { GoogleAuthAddress = x.Response.Headers["Location"], Success = true });
    }

    
}