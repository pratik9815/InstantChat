using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InstantChat.Application.Common.Interfaces;
using InstantChat.Domain.Interfaces;
using MediatR;

namespace InstantChat.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, GetChatHistoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public GetChatHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<GetChatHistoryResponse> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var messages = new List<Domain.Entities.ChatMessage>();
            int totalCount = 0;

            if (!string.IsNullOrEmpty(request.OtherUserId))
            {
                // Get private messages between two users (excluding soft-deleted messages)
                var allMessages = await _unitOfWork.Messages.FindAsync(m =>
                    !m.IsDeleted &&
                    ((m.SenderId == request.UserId && m.ReceiverId == request.OtherUserId) ||
                     (m.SenderId == request.OtherUserId && m.ReceiverId == request.UserId)));

                var orderedMessages = allMessages.OrderByDescending(m => m.Timestamp).ToList();
                totalCount = orderedMessages.Count;

                // Apply cursor-based pagination if BeforeMessageId is provided
                if (request.BeforeMessageId.HasValue)
                {
                    var beforeMessage = orderedMessages.FirstOrDefault(m => m.Id == request.BeforeMessageId.Value);
                    if (beforeMessage != null)
                    {
                        var beforeIndex = orderedMessages.IndexOf(beforeMessage);
                        messages = orderedMessages
                            .Skip(beforeIndex + 1)
                            .Take(request.PageSize)
                            .ToList();
                    }
                }
                else
                {
                    // Initial load - get the most recent messages
                    messages = orderedMessages
                        .Skip(request.Offset)
                        .Take(request.PageSize)
                        .ToList();
                }
            }
            else if (request.GroupChatId.HasValue)
            {
                // Get group messages (excluding soft-deleted messages)
                var allMessages = await _unitOfWork.Messages.FindAsync(m =>
                    !m.IsDeleted && m.GroupChatId == request.GroupChatId.Value);
                var orderedMessages = allMessages.OrderByDescending(m => m.Timestamp).ToList();
                totalCount = orderedMessages.Count;

                // Apply cursor-based pagination if BeforeMessageId is provided
                if (request.BeforeMessageId.HasValue)
                {
                    var beforeMessage = orderedMessages.FirstOrDefault(m => m.Id == request.BeforeMessageId.Value);
                    if (beforeMessage != null)
                    {
                        var beforeIndex = orderedMessages.IndexOf(beforeMessage);
                        messages = orderedMessages
                            .Skip(beforeIndex + 1)
                            .Take(request.PageSize)
                            .ToList();
                    }
                }
                else
                {
                    // Initial load - get the most recent messages
                    messages = orderedMessages
                        .Skip(request.Offset)
                        .Take(request.PageSize)
                        .ToList();
                }
            }

            // Reverse the order so oldest messages appear first in the UI
            messages = messages.OrderBy(m => m.Timestamp).ToList();

            var messageDtos = _mapper.Map<List<ChatMessageDto>>(messages);

            // Set image URLs for messages with images
            foreach (var dto in messageDtos.Where(m => !string.IsNullOrEmpty(m.ImagePath)))
            {
                dto.ImageUrl = _fileStorageService.GetFileUrl(dto.ImagePath!);
            }

            // Determine if there are more messages to load
            bool hasMoreMessages = false;
            if (request.BeforeMessageId.HasValue)
            {
                // Check if there are older messages before the current batch
                var olderMessagesQuery = !string.IsNullOrEmpty(request.OtherUserId)
                    ? await _unitOfWork.Messages.FindAsync(m =>
                        !m.IsDeleted &&
                        ((m.SenderId == request.UserId && m.ReceiverId == request.OtherUserId) ||
                         (m.SenderId == request.OtherUserId && m.ReceiverId == request.UserId)) &&
                        m.Timestamp < messages.First().Timestamp)
                    : await _unitOfWork.Messages.FindAsync(m =>
                        !m.IsDeleted &&
                        m.GroupChatId == request.GroupChatId.Value &&
                        m.Timestamp < messages.First().Timestamp);

                hasMoreMessages = olderMessagesQuery.Any();
            }
            else
            {
                hasMoreMessages = (request.Offset + messages.Count) < totalCount;
            }

            return new GetChatHistoryResponse
            {
                Messages = messageDtos,
                Success = true,
                Message = "Chat history retrieved successfully",
                HasMoreMessages = hasMoreMessages,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            return new GetChatHistoryResponse
            {
                Success = false,
                Message = $"Error retrieving chat history: {ex.Message}",
                HasMoreMessages = false,
                TotalCount = 0
            };
        }
    }
}
