using System.Security.Claims;
using InstantChat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using InstantChat.Application.Features.Chat.Commands.SendMessage;

namespace InstantChat.Infrastructure.Hubs;
[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;
    private static readonly Dictionary<string, string> _connectionUserMap = new();

    public ChatHub(UserManager<ApplicationUser> userManager, IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    public override async Task OnConnectedAsync()
    {
        var user = await _userManager.GetUserAsync(Context.User!);
        if (user != null)
        {
            _connectionUserMap[Context.ConnectionId] = user.Id;
            user.UpdateOnlineStatus(true);
            await _userManager.UpdateAsync(user);

            await Clients.Others.UserConnected(user.Id, user.DisplayName);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connectionUserMap.TryGetValue(Context.ConnectionId, out var userId))
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.UpdateOnlineStatus(false);
                await _userManager.UpdateAsync(user);

                await Clients.Others.UserDisconnected(user.Id, user.DisplayName);
            }

            _connectionUserMap.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToUser(string receiverId, string message)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return;

        var command = new SendMessageCommand
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = message
        };

        await _mediator.Send(command);
    }

    public async Task SendMessageToGroup(int groupId, string message)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return;

        var command = new SendMessageCommand
        {
            SenderId = senderId,
            GroupChatId = groupId,
            Content = message
        };

        await _mediator.Send(command);
    }

    public async Task JoinGroup(int groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"GroupChat_{groupId}");
    }

    public async Task LeaveGroup(int groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"GroupChat_{groupId}");
    }

    public async Task NotifyTyping(string? receiverId, int? groupId, bool isTyping)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return;

        var sender = await _userManager.FindByIdAsync(senderId);
        if (sender == null) return;

        if (receiverId != null)
        {
            await Clients.User(receiverId).NotifyTyping(senderId, sender.DisplayName, isTyping);
        }
        else if (groupId.HasValue)
        {
            await Clients.Group($"GroupChat_{groupId.Value}")
                .NotifyTyping(senderId, sender.DisplayName, isTyping);
        }
    }
}
