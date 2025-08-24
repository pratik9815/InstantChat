using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;

public class GetOnlineUsersQuery : IRequest<GetOnlineUsersResponse>
{
    public string CurrentUserId { get; set; } = string.Empty;
}

public class GetOnlineUsersResponse
{
    public List<UserDto> Users { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}
