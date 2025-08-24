using InstantChat.Domain.Common;

namespace InstantChat.Domain.Entities;

public class ChatMessage : BaseAuditableEntity
{
    public string SenderId { get; private set; } = string.Empty;
    public string? ReceiverId { get; private set; }
    public int? GroupChatId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public bool IsRead { get; private set; }
    public MessageType Type { get; private set; }
    public string? ImagePath { get; private set; } // For image messages
    public string? OriginalFileName { get; private set; } // Original file name
    public long? FileSize { get; private set; } // File size in bytes

    // Navigation properties
    public virtual ApplicationUser Sender { get; private set; } = null!;
    public virtual ApplicationUser? Receiver { get; private set; }
    public virtual GroupChat? GroupChat { get; private set; }

    public ChatMessage() { }

    public ChatMessage(string senderId, string content, string? receiverId = null, int? groupChatId = null)
    {
        SenderId = senderId;
        Content = content;
        ReceiverId = receiverId;
        GroupChatId = groupChatId;
        Timestamp = DateTime.UtcNow;
        Type = receiverId != null ? MessageType.Private : MessageType.Group;
        SetCreatedBy(senderId);
    }

    public ChatMessage(string senderId, string imagePath, string originalFileName, long fileSize,
        string? receiverId = null, int? groupChatId = null, string? caption = null)
    {
        SenderId = senderId;
        ImagePath = imagePath;
        OriginalFileName = originalFileName;
        FileSize = fileSize;
        Content = caption ?? string.Empty;
        ReceiverId = receiverId;
        GroupChatId = groupChatId;
        Timestamp = DateTime.UtcNow;
        Type = MessageType.Image;
        SetCreatedBy(senderId);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void UpdateContent(string content, string updatedBy)
    {
        Content = content;
        SetUpdatedBy(updatedBy);
    }

    public bool IsImageMessage() => Type == MessageType.Image && !string.IsNullOrEmpty(ImagePath);
}

public enum MessageType
{
    Private,
    Group,
    Image
}