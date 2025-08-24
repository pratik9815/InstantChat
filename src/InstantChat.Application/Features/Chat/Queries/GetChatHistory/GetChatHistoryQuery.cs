using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InstantChat.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQuery : IRequest<GetChatHistoryResponse>
{
    public string UserId { get; set; } = string.Empty;
    public string? OtherUserId { get; set; }
    public int? GroupChatId { get; set; }
    public int PageSize { get; set; } = 20;
    public int Offset { get; set; } = 0;
    public int? BeforeMessageId { get; set; } // For cursor-based pagination
}

public class GetChatHistoryResponse
{
    public List<ChatMessageDto> Messages { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool HasMoreMessages { get; set; }
    public int TotalCount { get; set; }
}

public class ChatMessageDto
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
    public MessageType Type { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
    public string? OriginalFileName { get; set; }
    public long? FileSize { get; set; }
}

public enum MessageType
{
    Private,
    Group,
    Image
}
