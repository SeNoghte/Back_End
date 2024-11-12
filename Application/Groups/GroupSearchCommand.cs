using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace Application.Groups
{
    public class GroupSearchCommand : IRequest<GroupSearchResult>
    {
        public string Name { get; set; }
        public string SearcherId { get; set; }
    }

    public class GroupSearchResult : ResultModel
    {
        public List<Group> FilteredGroups { get; set; }
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

            if (request.Name == null)
            {
                result.FilteredGroups = await dBContext.Groups.ToListAsync();
            }

            else
            {
                result.FilteredGroups = await dBContext.Groups
                    .Where(gp => gp.Name.Contains(request.Name))
                    .ToListAsync();
            }

            result.Success = true;
            return result;
        }
    }
}
