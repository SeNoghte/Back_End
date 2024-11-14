using Application.Common.Models;
using Application.Common.Services.IdentityService;
using Application.Groups;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class ProfileInfoCommand : IRequest<ProfileInfoResult>
    {

    }

    public class ProfileInfoResult : ResultModel
    {
        public string Name { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public DateTime JoinedDate { get; set; }
    }

    public class ProfileInfoHandler : IRequestHandler<ProfileInfoCommand, ProfileInfoResult>
    {
        private readonly IIdentityService identityService;
        private readonly ApplicationDBContext dBContext;

        public ProfileInfoHandler(IIdentityService identityService, ApplicationDBContext dBContext)
        {
            this.identityService = identityService;
            this.dBContext = dBContext;
        }

        public async Task<ProfileInfoResult> Handle(ProfileInfoCommand request, CancellationToken cancellationToken)
        {
            var result = new ProfileInfoResult();

            var UserId = identityService.GetCurrentUserId();

            if (UserId == null)
            {
                result.ErrorCode = 401;
                result.Message = "Unauthorized";
                return result;
            }

            var user = await dBContext.Users.Where(u => u.UserId == UserId).FirstOrDefaultAsync();
            
            if(user == null)
            {
                result.Message = "کاربر پیدا نشد";
                result.ErrorCode = 404;
                return result;
            }

            result.Name = user.Name;
            result.Email = user.Email;
            result.Username = user.Username;
            result.JoinedDate = user.JoinedDate;
            result.Success = true;

            return result;
        }
    }
}
