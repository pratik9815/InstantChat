using InstantChat.Application.Common.Interfaces;
using InstantChat.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace InstantChat.Infrastructure.Services;

public class ChatHubService : IChatHubService
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public ChatHubService(IHubContext<ChatHub, IChatClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToUserAsync(string userId, string senderId, string senderName, string message, string timestamp, string? imagePath = null)
    {
        await _hubContext.Clients.User(userId).ReceiveMessage(senderId, senderName, message, timestamp);
        await _hubContext.Clients.User(senderId).ReceiveMessage(senderId, senderName, message, timestamp);
    }

    public async Task SendMessageToGroupAsync(int groupId, string senderId, string senderName, string message, string timestamp, string? imagePath = null)
    {
        await _hubContext.Clients.Group($"GroupChat_{groupId}")
            .ReceiveGroupMessage(groupId, "", senderId, senderName, message, timestamp);
    }

    public async Task NotifyUserConnectedAsync(string userId, string userName)
    {
        await _hubContext.Clients.All.UserConnected(userId, userName);
    }

    public async Task NotifyUserDisconnectedAsync(string userId, string userName)
    {
        await _hubContext.Clients.All.UserDisconnected(userId, userName);
    }

    public async Task NotifyTypingAsync(string? userId, int? groupId, string typingUserId, string typingUserName, bool isTyping)
    {
        if (userId != null)
        {
            await _hubContext.Clients.User(userId).NotifyTyping(typingUserId, typingUserName, isTyping);
        }
        else if (groupId.HasValue)
        {
            await _hubContext.Clients.Group($"GroupChat_{groupId.Value}")
                .NotifyTyping(typingUserId, typingUserName, isTyping);
        }
    }
}
