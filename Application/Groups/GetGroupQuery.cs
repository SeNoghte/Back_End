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
                        .Include(g => g.Members)         
                        .ThenInclude(ug => ug.User)      
                        .FirstOrDefaultAsync();

                if (group == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "گروه پیدا نشد";
                    return result;
                }

                var groupDto = new GroupDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    CreatedDate = group.CreatedDate,
                    Image = group.Image,
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

                result.Group = groupDto;
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
