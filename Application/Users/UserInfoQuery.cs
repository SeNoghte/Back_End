using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.DTO;
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
    public class UserInfoQuery : IRequest<UserInfoResult>
    {
        public Guid UserId { get; set; }
    }

    public class UserInfoResult : ResultModel
    {
        public UserDto User { get; set; }
        public List<GroupDto> GroupInCommon { get; set; }
    }

    public class UserInfoHandler : IRequestHandler<UserInfoQuery, UserInfoResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IIdentityService identityService;
        private readonly IGeneralServices generalServices;

        public UserInfoHandler(ApplicationDBContext dBContext, IIdentityService identityService, IGeneralServices generalServices)
        {
            this.dBContext = dBContext;
            this.identityService = identityService;
            this.generalServices = generalServices;
        }

        public async Task<UserInfoResult> Handle(UserInfoQuery request, CancellationToken cancellationToken)
        {
            var result = new UserInfoResult();

            try
            {
                var currentUserId = identityService.GetCurrentUserId();

                if (currentUserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                
                var user = await dBContext.Users
                   .Include(u => u.Groups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

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

                
                var currentUserGroups = await dBContext.UserGroups
                    .Where(ug => ug.UserId == currentUserId)
                    .Select(ug => ug.GroupId)
                    .ToListAsync(cancellationToken);

                
                result.GroupInCommon = user.Groups
                    .Where(ug => currentUserGroups.Contains(ug.GroupId))
                    .Select(ug => new GroupDto
                    {
                        Id = ug.Group.Id,
                        Name = ug.Group.Name,
                        Image = ug.Group.Image
                    })
                    .ToList();

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
