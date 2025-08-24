using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using MediatR;

namespace InstantChat.Application.Features.Groups.Commands.CreateGroup;
public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, CreateGroupResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var group = new GroupChat(request.Name, request.CreatedById, request.Description);
            await _unitOfWork.Groups.AddAsync(group);
            await _unitOfWork.SaveChangesAsync();

            // Add creator as participant
            var participant = new GroupChatParticipant(group.Id, request.CreatedById);
            await _unitOfWork.GroupParticipants.AddAsync(participant);
            await _unitOfWork.SaveChangesAsync();

            return new CreateGroupResponse
            {
                Success = true,
                Message = "Group created successfully",
                GroupId = group.Id
            };
        }
        catch (Exception ex)
        {
            return new CreateGroupResponse
            {
                Success = false,
                Message = $"Error creating group: {ex.Message}"
            };
        }
    }
}
