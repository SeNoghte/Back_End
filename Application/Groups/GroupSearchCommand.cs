using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        public GroupSearchHandler(ApplicationDBContext dBContext, IGeneralServices generalServices)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
        }

        public async Task<GroupSearchResult> Handle(GroupSearchCommand request, CancellationToken cancellationToken)
        {
            var result = new GroupSearchResult();

            var userExist = await generalServices.CheckUserExists(request.SearcherId);

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
