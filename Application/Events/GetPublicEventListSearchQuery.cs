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

namespace Application.Events;

public class GetPublicEventListSearchQuery:IRequest<GetPublicEventListSearchResult>
{
    public string? SearchString { get; set; }
}

public class GetPublicEventListSearchResult:ResultModel
{
    public List<GetPublicEventListSearchModel> Items { get; set; }

}

public class GetPublicEventListSearchModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string OwnerName { get; set; }
    public string? OwnerImage { get; set; }
    public string Date { get; set; }
    public string? Time { get; set; }
    public string ImagePath { get; set; }
}

public class GetPublicEventListSearchHandler : IRequestHandler<GetPublicEventListSearchQuery, GetPublicEventListSearchResult>
{
    private ApplicationDBContext dBContext;
    IIdentityService identityService;
    public GetPublicEventListSearchHandler(ApplicationDBContext dBContext, IIdentityService identityService)
    {
        this.dBContext = dBContext;
        this.identityService = identityService;
    }

    public async Task<GetPublicEventListSearchResult> Handle(GetPublicEventListSearchQuery request, CancellationToken cancellationToken)
    {
        var result = new GetPublicEventListSearchResult();
        var currentUserId = identityService.GetCurrentUserId();

        var query = dBContext.Events.Include(x => x.EventMembers).Include(x => x.Owner)
            .Where(x => !x.IsPrivate);

        if (!string.IsNullOrEmpty(request.SearchString))
        {
            string search = request.SearchString.ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(search));
        }

        result.Items = await query.OrderByDescending(x => x.EventMembers.Count())
            .Select(x => new GetPublicEventListSearchModel()
            {
                Date = x.StartDate.ToString("yyyy-MM-dd"),
                Time = x.StartDate.ToString("HH:mm:ss"),
                Description = x.Description,
                Id = x.Id,
                ImagePath = x.ImagePath,
                OwnerImage = x.Owner.Image,
                OwnerName = x.Owner.Name,
                Title = x.Title
            }).ToListAsync();

        result.Success = true;
        return result;
    }
}