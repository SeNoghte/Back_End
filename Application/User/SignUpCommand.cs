using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User
{
    public class SignUpCommand : RequestModel<SignUpResult>
    {
        
    }

    public class SignUpResult : ResultModel
    {
        public string? GoogleAuthAddress { get; set; }
    }


    public class SignUpHandler : IRequestHandler<SignUpCommand, SignUpResult>
    {
        public async Task<SignUpResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var result = new SignUpResult();
            var properties = new AuthenticationProperties { RedirectUri =  "/"};
            result.Success = true;      
            return result;
            

        }
    }

}
