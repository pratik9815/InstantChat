using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InstantChat.Application.Features.Groups.Queries.GetUserGroups;

public class GetUserGroupsQuery : IRequest<GetUserGroupsResponse>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetUserGroupsResponse
{
    public List<GroupChatDto> Groups { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class GroupChatDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ParticipantCount { get; set; }
}