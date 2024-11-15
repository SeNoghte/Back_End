using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.DTO;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Application.Groups
{
    public class GroupSearchCommand : IRequest<GroupSearchResult>
    {
        public string Filter { get; set; }
    }

    public class GroupSearchResult : ResultModel
    {
        public List<GroupDto> FilteredGroups { get; set; }
    }

    public class GroupSearchHandler : IRequestHandler<GroupSearchCommand, GroupSearchResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;
        IIdentityService identityService {  get; set; }

        public GroupSearchHandler(ApplicationDBContext dBContext, IGeneralServices generalServices,
            IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
            this.identityService = identityService;
        }

        public async Task<GroupSearchResult> Handle(GroupSearchCommand request, CancellationToken cancellationToken)
        {
            var result = new GroupSearchResult();

            var UserId= identityService.GetCurrentUserId();

            if (UserId == null)
            {
                result.ErrorCode = 401;
                result.Message = "Unauthorized";
                return result;
            }

            var userExist = await generalServices.CheckUserExists((Guid)UserId);

            if (!userExist)
            {
                result.Message = "کاربر پیدا نشد";
                return result;
            }

            var groupsQuery = dBContext.Groups
                .Include(g => g.Members)              
                .ThenInclude(ug => ug.User)            
                .OrderBy(g => g.Name);

            if (request.Filter != null)
            {
                groupsQuery = groupsQuery.Where(g => g.Name.Contains(request.Filter)).OrderBy(gp => gp.Name);
            }

            var groups = await groupsQuery.ToListAsync();

            result.FilteredGroups = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreatedDate = g.CreatedDate,
                Owner = new UserDto
                {
                    UserId = g.OwnerId,
                    Name = g.Owner.Name,
                    Username = g.Owner.Username,
                    Email = g.Owner.Email,
                    JoinedDate = g.Owner.JoinedDate,
                },
                Members = g.Members.Select(m => new UserDto
                {
                    UserId = g.OwnerId,
                    Name = m.User.Name,
                    Username = m.User.Username,
                    Email = m.User.Email,
                    JoinedDate = m.User.JoinedDate,
                }).ToList()
            }).ToList();

            result.Success = true;
            return result;
        }
    }
}
