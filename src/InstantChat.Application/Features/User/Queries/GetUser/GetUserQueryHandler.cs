using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;
using InstantChat.Domain.Interfaces;
using MediatR;

namespace InstantChat.Application.Features.User.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, List<GetUserCommandResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<List<GetUserCommandResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if(request.name == null)
            return new List<GetUserCommandResponse>();
        var users = await _userRepository.GetUserIdByNameAsync(request.name);
        if(users.Count == 0)
            return new List<GetUserCommandResponse>();
        var mappedUsers = _mapper.Map<List<UserDto>>(users);
        var response = mappedUsers.Select(u => new GetUserCommandResponse
        {
            UserId = u.Id,
            Name = u.DisplayName,
        }).ToList();
        return response;
    }
}
