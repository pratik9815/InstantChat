using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstantChat.Application.Common.Interfaces;
using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace InstantChat.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IChatHubService _chatHubService;
    private readonly IFileStorageService _fileStorageService;

    public SendMessageCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IChatHubService chatHubService,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _chatHubService = chatHubService;
        _fileStorageService = fileStorageService;
    }

    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _userManager.FindByIdAsync(request.SenderId).ConfigureAwait(false);
        if (sender == null)
        {
            return new SendMessageResponse { Success = false, Message = "Sender not found" };
        }

        // Validate receiver or group
        if (!string.IsNullOrEmpty(request.ReceiverId))
        {
            var receiver = await _userManager.FindByIdAsync(request.ReceiverId);
            if (receiver == null)
            {
                return new SendMessageResponse { Success = false, Message = "Receiver not found" };
            }
        }
        else if (request.GroupChatId.HasValue)
        {
            var group = await _unitOfWork.Groups.FirstOrDefaultAsync(g =>
                g.Id == request.GroupChatId.Value && !g.IsDeleted);
            if (group == null)
            {
                return new SendMessageResponse { Success = false, Message = "Group not found" };
            }

            // Check if sender is an active participant
            var participation = await _unitOfWork.GroupParticipants
                .FirstOrDefaultAsync(p => p.GroupChatId == request.GroupChatId.Value &&
                                         p.UserId == request.SenderId &&
                                         !p.IsDeleted);

            if (participation == null)
            {
                return new SendMessageResponse { Success = false, Message = "User is not a member of this group" };
            }
        }

        ChatMessage message;
        string? imagePath = null;

        // Handle image message
        if (request.ImageFile != null)
        {
            try
            {
                if (!_fileStorageService.IsValidImageFile(request.ImageFile.FileName, request.ImageFile.Length))
                {
                    return new SendMessageResponse { Success = false, Message = "Invalid image file" };
                }

                using var stream = request.ImageFile.OpenReadStream();
                imagePath = await _fileStorageService.SaveFileAsync(stream, request.ImageFile.FileName, "chat-images");

                message = new ChatMessage(
                    request.SenderId,
                    imagePath,
                    request.ImageFile.FileName,
                    request.ImageFile.Length,
                    request.ReceiverId,
                    request.GroupChatId,
                    request.Content 
                );
            }
            catch (Exception ex)
            {
                return new SendMessageResponse { Success = false, Message = $"Failed to save image: {ex.Message}" };
            }
        }
        else
        {
            // Handle text message
            message = new ChatMessage(request.SenderId, request.Content, request.ReceiverId, request.GroupChatId);
        }

        await _unitOfWork.Messages.AddAsync(message).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Send real-time notification
        var timestamp = message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        var imageUrl = imagePath != null ? _fileStorageService.GetFileUrl(imagePath) : null;

        if (!string.IsNullOrEmpty(request.ReceiverId))
        {
            await _chatHubService.SendMessageToUserAsync(
                request.ReceiverId, request.SenderId, sender.DisplayName,
                request.Content, timestamp, imageUrl);
        }
        else if (request.GroupChatId.HasValue)
        {
            await _chatHubService.SendMessageToGroupAsync(
                request.GroupChatId.Value, request.SenderId, sender.DisplayName,
                request.Content, timestamp, imageUrl);
        }

        return new SendMessageResponse
        {
            Success = true,
            Message = "Message sent successfully",
            MessageId = message.Id,
            ImagePath = imageUrl,
            MessageType = message.IsImageMessage() ? MessageType.Image : MessageType.Text
        };
    }
}