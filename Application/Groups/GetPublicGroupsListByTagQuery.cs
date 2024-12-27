using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Groups;

public class GetPublicGroupsListByTagQuery : IRequest<GetPublicGroupsListByTagResult>
{
    public string? Tag { get; set; }
}

public class GetPublicGroupsListByTagResult : ResultModel
{
    public List<GetPublicGroupsListByTagModel> Items { get; set; }
}

public class GetPublicGroupsListByTagModel
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
}
public class GetPublicGroupsListByTagHandler : IRequestHandler<GetPublicGroupsListByTagQuery, GetPublicGroupsListByTagResult>
{
    private ApplicationDBContext dBContext;
    IIdentityService identityService;
    public GetPublicGroupsListByTagHandler(ApplicationDBContext dBContext, IIdentityService identityService)
    {
        this.dBContext = dBContext;
        this.identityService = identityService;
    }
    async Task<GetPublicGroupsListByTagResult> IRequestHandler<GetPublicGroupsListByTagQuery, GetPublicGroupsListByTagResult>.Handle(GetPublicGroupsListByTagQuery request, CancellationToken cancellationToken)
    {
        var result = new GetPublicGroupsListByTagResult();

        var currentUserId = identityService.GetCurrentUserId();

        var query = dBContext.Groups.Include(x => x.Members).Include(x => x.Events).ThenInclude(x => x.Tags).Where(x => !x.IsPrivate && !x.Members.Any(y => y.UserId == currentUserId));

        if (!string.IsNullOrEmpty(request.Tag))
        {
            query = query.Where(x => x.Events.Any(y => y.Tags != null && y.Tags.Any(z => z.Tag == request.Tag)));
        }

        result.Items = await query.OrderByDescending(x => x.Members.Count()).Take(3)
            .Select(x => new GetPublicGroupsListByTagModel
            {
                Description = x.Description,
                Id = x.Id,
                Image = x.Image,
                Name = x.Name
            }).ToListAsync();

        result.Success = true;

        return result;
    }

}