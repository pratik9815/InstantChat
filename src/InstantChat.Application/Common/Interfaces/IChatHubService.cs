using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantChat.Application.Common.Interfaces;

public interface IChatHubService
{
    Task SendMessageToUserAsync(string userId, string senderId, string senderName, string message, string timestamp, string? imagePath = null);
    Task SendMessageToGroupAsync(int groupId, string senderId, string senderName, string message, string timestamp, string? imagePath = null);
    Task NotifyUserConnectedAsync(string userId, string userName);
    Task NotifyUserDisconnectedAsync(string userId, string userName);
    Task NotifyTypingAsync(string? userId, int? groupId, string typingUserId, string typingUserName, bool isTyping);
}
