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

public class GetPublicGroupListSearchQuery : IRequest<GetPublicGroupListSearchResult>
{
    public string? SearchString { get; set; }
}

public class GetPublicGroupListSearchResult : ResultModel
{
    public List<GetPublicGroupListSearchModel> Items { get; set; }
}

public class GetPublicGroupListSearchModel
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
}
public class GetPublicGroupListSearchHandler : IRequestHandler<GetPublicGroupListSearchQuery, GetPublicGroupListSearchResult>
{
    private ApplicationDBContext dBContext;
    IIdentityService identityService;
    public GetPublicGroupListSearchHandler(ApplicationDBContext dBContext, IIdentityService identityService)
    {
        this.dBContext = dBContext;
        this.identityService = identityService;
    }
    async Task<GetPublicGroupListSearchResult> IRequestHandler<GetPublicGroupListSearchQuery, GetPublicGroupListSearchResult>.Handle(GetPublicGroupListSearchQuery request, CancellationToken cancellationToken)
    {
        var result = new GetPublicGroupListSearchResult();

        var currentUserId = identityService.GetCurrentUserId();

        var query = dBContext.Groups.Include(x => x.Members).Include(x => x.Events).ThenInclude(x => x.Tags).Where(x => !x.IsPrivate);

        if (!string.IsNullOrEmpty(request.SearchString))
        {
            string search = request.SearchString.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search));
        }

        result.Items = await query.OrderByDescending(x => x.Members.Count())
            .Select(x => new GetPublicGroupListSearchModel
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