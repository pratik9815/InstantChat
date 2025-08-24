using MediatR;

namespace InstantChat.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommand : IRequest<CreateGroupResponse>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CreatedById { get; set; } = string.Empty;
}

public class CreateGroupResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? GroupId { get; set; }
}
