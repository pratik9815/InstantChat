using MediatR;
using Microsoft.AspNetCore.Http;

namespace InstantChat.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommand : IRequest<SendMessageResponse>
{
    public string SenderId { get; set; } = string.Empty;
    public string? ReceiverId { get; set; }
    public int? GroupChatId { get; set; }
    public string Content { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; } // For image messages
}

public class SendMessageResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? MessageId { get; set; }
    public string? ImagePath { get; set; }
    public MessageType MessageType { get; set; }
}

public enum MessageType
{
    Text,
    Image
}