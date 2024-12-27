using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events;

public class GetPublicEventsListByTagQuery : IRequest<GetPublicEventListByTagResult>
{
    public string? Tag { get; set; }
}

public class GetPublicEventListByTagResult : ResultModel
{
    public List<GetPublicEventsListByTagModel> Items { get; set; }
}

public class GetPublicEventsListByTagModel
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
public class GetPublicEventsListByTagHandler : IRequestHandler<GetPublicEventsListByTagQuery, GetPublicEventListByTagResult>
{
    private ApplicationDBContext dBContext;
    IIdentityService identityService;
    public GetPublicEventsListByTagHandler(ApplicationDBContext dBContext, IIdentityService identityService)
    {
        this.dBContext = dBContext;
        this.identityService = identityService;
    }

    public async Task<GetPublicEventListByTagResult> Handle(GetPublicEventsListByTagQuery request, CancellationToken cancellationToken)
    {
        var result = new GetPublicEventListByTagResult();

        var currentUserId = identityService.GetCurrentUserId();

        var query = dBContext.Events.Include(x => x.EventMembers).Include(x => x.Owner)
            .Where(x => !x.IsPrivate && x.EventMembers.Count() < x.MaxMembers && !x.EventMembers.Any(y => y.UserId == currentUserId));

        if (!string.IsNullOrEmpty(request.Tag))
        {
            query = query.Where(x => x.Tags != null && x.Tags.Any(x => x.Tag == request.Tag));
        }

        result.Items = await query.OrderByDescending(x => x.EventMembers.Count())
            .Select(x => new GetPublicEventsListByTagModel()
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