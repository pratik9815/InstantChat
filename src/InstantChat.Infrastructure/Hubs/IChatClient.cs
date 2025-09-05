using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantChat.Infrastructure.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(string senderId, string senderName, string message, string timestamp, string imagePath);
    Task ReceiveGroupMessage(int groupId, string groupName, string senderId, string senderName, string message, string timestamp);
    Task UserConnected(string userId, string userName);
    Task UserDisconnected(string userId, string userName);
    Task JoinedGroup(int groupId, string groupName);
    Task LeftGroup(int groupId, string groupName);
    Task NotifyTyping(string userId, string userName, bool isTyping);
    Task ReceiveImageMessage(string senderId, string imageUrl, string? caption);
}
