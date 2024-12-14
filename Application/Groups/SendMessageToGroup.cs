using Domain.Entities;
using MediatR;

using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Application.ChatHubs;
using DataAccess;
using Application.Common.Models;
using Application.Common.Services.IdentityService;

namespace Application.Groups;

public class SendMessageToGroup : IRequest<SendMessageToGroupResult>
{
    public string Text { get; set; }
    public Guid GroupId { get; set; }
}
public class SendMessageToGroupResult : ResultModel
{
}

public class SendMessageToGroupHandler : IRequestHandler<SendMessageToGroup, SendMessageToGroupResult>
{
    private IHubContext<ChatHub> _messageHub;
    ApplicationDBContext dBContext;
    IIdentityService identityService;

    public SendMessageToGroupHandler(IHubContext<ChatHub> messageHub, ApplicationDBContext dBContext,
        IIdentityService identityService)
    {
        _messageHub = messageHub;
        this.dBContext = dBContext;
        this.identityService = identityService;
    }


    public async Task<SendMessageToGroupResult> Handle(SendMessageToGroup request, CancellationToken cancellationToken)
    {
        var currentUserId = identityService.GetCurrentUserId();
        var result = new SendMessageToGroupResult();
        var group = await dBContext.Groups.Include(x => x.Members).FirstOrDefaultAsync(g => g.Id == request.GroupId);

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

        var user = await dBContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

        var newMessage = new Message
        {
            Text = request.Text,
            SentTime = DateTime.UtcNow,
            UserId = user.UserId,
            GroupId = group.Id
        };

        dBContext.Messages.Add(newMessage);
        await dBContext.SaveChangesAsync();

        var messageForFront = new
        {
            userName = user.Name,
            text = request.Text,
            sentTime = newMessage.SentTime,
            userId = newMessage.UserId.ToString(),
            isMe = false
        };

        await _messageHub.Clients.Groups(request.GroupId.ToString()).SendAsync("ReceiveMessage", messageForFront);
        
        result.Success = true;
        return result;

    }
}