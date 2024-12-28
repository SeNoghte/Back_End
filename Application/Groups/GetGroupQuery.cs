using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.DTO;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Groups
{
    public class GetGroupQuery : IRequest<GetGroupResult>
    {
        public Guid GroupId { get; set; }
    }

    public class GetGroupResult : ResultModel
    {
        public GroupDto Group { get; set; }
        public List<EventDto> Events { get; set; }
    }

    public class GetGroupHandler : IRequestHandler<GetGroupQuery, GetGroupResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;
        private readonly IIdentityService identityService;

        public GetGroupHandler(ApplicationDBContext dBContext, IGeneralServices generalServices,
            IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
            this.identityService = identityService;
        }

        public async Task<GetGroupResult> Handle(GetGroupQuery request, CancellationToken cancellationToken)
        {
            var result = new GetGroupResult();

            try
            {
                var userId = identityService.GetCurrentUserId();

                if (userId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var group = await dBContext.Groups
                                .Where(g => g.Id == request.GroupId)
                                .Include(g => g.Owner) 
                                .Include(g => g.Events) 
                                    .ThenInclude(e => e.EventMembers) 
                                .Include(g => g.Events)
                                    .ThenInclude(e => e.Owner) 
                                .Include(g => g.Members)
                                    .ThenInclude(ug => ug.User)
                                .FirstOrDefaultAsync();


                if (group == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "گروه پیدا نشد";
                    return result;
                }


                var isCurrentUserAMember = await dBContext.UserGroups.AnyAsync(ug => ug.UserId == userId && ug.GroupId == group.Id);

                result.Group= new GroupDto
                {
                    Id = group.Id,
                    IsPrivate = group.IsPrivate,
                    Name = group.Name,
                    Description = group.Description,
                    CreatedDate = group.CreatedDate,                 
                    Image = group.Image,
                    IsAdmin = userId == group.Owner.UserId,
                    IsMember = isCurrentUserAMember,
                    Owner = new UserDto
                    {
                        UserId = group.Owner.UserId,
                        Name = group.Owner.Name,
                        Username = group.Owner.Username,
                        Email = group.Owner.Email,
                        JoinedDate = group.Owner.JoinedDate,
                        Image = group.Owner.Image
                    },
                    Members = group.Members.Select(ug => new UserDto
                    {
                        UserId = ug.User.UserId,
                        Name = ug.User.Name,
                        Username = ug.User.Username,
                        Email = ug.User.Email,
                        JoinedDate = ug.User.JoinedDate,
                        Image = ug.User.Image
                    }).ToList()
                };

                result.Events = group.Events.OrderBy(e => e.StartDate).Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.StartDate.ToString(),
                    IsPrivate =e.IsPrivate,
                    Owner = new UserDto { 
                        UserId = e.Owner.UserId,
                        Name = e.Owner.Name,
                        Image = e.Owner.Image,
                    },
                    ImagePath = e.ImagePath,                                    
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
