using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.IdentityService;
using Application.DTO;
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
        public UserDto User { get; set; }
        public List<GroupDto> MyGroups { get; set; }
    }

    public class ProfileInfoHandler : IRequestHandler<ProfileInfoCommand, ProfileInfoResult>
    {
        private readonly IIdentityService identityService;
        private readonly ApplicationDBContext dBContext;
        private readonly ICloudService cloudService;

        public ProfileInfoHandler(IIdentityService identityService, ApplicationDBContext dBContext, ICloudService cloudService)
        {
            this.identityService = identityService;
            this.dBContext = dBContext;
            this.cloudService = cloudService;
        }

        public async Task<ProfileInfoResult> Handle(ProfileInfoCommand request, CancellationToken cancellationToken)
        {
            var result = new ProfileInfoResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var user = await dBContext.Users
                .Include(u => u.Groups)
                    .ThenInclude(ug => ug.Group)
                .FirstOrDefaultAsync(u => u.UserId == UserId);

                if (user == null)
                {
                    result.Message = "کاربر پیدا نشد";
                    result.ErrorCode = 404;
                    return result;
                }

                result.User = new UserDto()
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Username = user.Username,
                    JoinedDate = user.JoinedDate,
                    Image = user.Image,
                };

                result.MyGroups = user.Groups.Select(g => new GroupDto
                {
                    Id = g.Group.Id,
                    Name = g.Group.Name,
                    Image = g.Group.Image
                }).ToList();

                result.Success = true;

                return result;
            }
            catch
            {
                result.Message = "مشکلی پیش آمده است";
                result.ErrorCode = 500;
                return result;
            }
        }
    }
}
