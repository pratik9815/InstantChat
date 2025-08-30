using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InstantChat.Application.Features.User.Queries.GetUser;

public record GetUserQuery(string name) : IRequest<List<GetUserCommandResponse>>;
public class GetUserCommandResponse
{
    public string UserId { get; set; }
    public string Name { get; set; }
}
