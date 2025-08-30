using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using MediatR;

namespace InstantChat.Application.Features.Groups.Commands.AddUser;
public class AddUserToGroupCommandHandler : IRequestHandler<AddUserToGroupCommand, AddUserToGroupResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddUserToGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<AddUserToGroupResponse> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            return new AddUserToGroupResponse
            {
                Success = false,
                Message = "Group not found"
            };
        }

        var existingParticipants = await _unitOfWork.GroupParticipants
            .FindAsync(p => p.GroupChatId == request.GroupId);

        // Filter out users who are already participants
        var usersToAdd = request.UserId
            .Where(userId => !existingParticipants.Any(p => p.UserId == userId))
            .ToList();
        if (!usersToAdd.Any())
        {
            return new AddUserToGroupResponse
            {
                Success = false,
                Message = "No new users to add. All selected users are already participants."
            };
        }
        // Create new participants
        var newParticipants = usersToAdd
            .Select(userId => new GroupChatParticipant(request.GroupId, userId))
            .ToList();

        await _unitOfWork.GroupParticipants.AddRangeAsync(newParticipants);
        await _unitOfWork.SaveChangesAsync();

        return new AddUserToGroupResponse
        {
            Success = true,
            Message = $"Users added to group successfully: {string.Join(", ", usersToAdd)}"
        };
    }

}
