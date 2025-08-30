using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InstantChat.Application.Features.User.Queries.GetUserById;

public record GetUserByIdQuery(string UserId) : IRequest<GetUserByIdResponse>;

public class GetUserByIdResponse
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}