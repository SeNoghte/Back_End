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

public class GetGroupMessageListQuery : IRequest<GetGroupMessageListResult>
{
    public Guid GroupId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetGroupMessageListResult : ResultModel
{
    public List<MessageModel> Items { get; set; }
}

public class MessageModel
{
    public string Text { get; set; }
    public string Username { get; set; }
    public DateTime SentTime { get; set; }
    public Guid UserId { get; set; }
}

public class GetGroupMessageListHandler : IRequestHandler<GetGroupMessageListQuery, GetGroupMessageListResult>
{
    ApplicationDBContext dBContext { get; set; }
    IIdentityService identityService { get; set; }
    public GetGroupMessageListHandler(ApplicationDBContext dBContext, IIdentityService identityService)
    {
        this.dBContext = dBContext;
        this.identityService = identityService;
    }

    public async Task<GetGroupMessageListResult> Handle(GetGroupMessageListQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = identityService.GetCurrentUserId();

        var group = await dBContext.Groups.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == request.GroupId);

        var result = new GetGroupMessageListResult();

        if (group == null)
        {
            result.Message = "گروه مورد نظر یافت نشد.";
            return result;
        }

        var userBlengToGroup = group.Members.Any(x => x.UserId == currentUserId);

        if (!userBlengToGroup)
        {
            result.Message = "شما به این گره دسترسی ندارید.";
            return result;
        }

        var messages = await dBContext.Messages
        .Where(m => m.GroupId == request.GroupId)
        .OrderByDescending(m => m.SentTime)
        .Skip((request.PageIndex - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(m => new MessageModel
        {
            Text = m.Text,
            SentTime = m.SentTime,
            Username = m.User.Name,
            UserId = m.UserId
        })
        .ToListAsync();


        result.Items = messages;
        result.Success = true;

        return result;
    }
}
