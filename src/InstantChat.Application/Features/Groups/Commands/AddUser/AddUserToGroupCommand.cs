using MediatR;

namespace InstantChat.Application.Features.Groups.Commands.AddUser;
public class AddUserToGroupCommand : IRequest<AddUserToGroupResponse>
{
    public int GroupId { get; set; }
    public List<string> UserId { get; set; } = new List<string>();
}

public class AddUserToGroupResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}