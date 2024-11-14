using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.ChatHubs;

public class ChatHub: Hub
{
    public void joinGroupHub(int groupId)
    {
        var connectionId = Context.ConnectionId;
        Groups.AddToGroupAsync(connectionId, groupId.ToString());
        Clients.Client(connectionId).SendAsync("joinGroup", $"You joined to group hub successfully");
    }

    public override Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        Clients.Client(connectionId).SendAsync("WelcomeMethodName", connectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        return base.OnDisconnectedAsync(exception);
    }
}
