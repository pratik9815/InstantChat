using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;
using MediatR;

namespace InstantChat.Application.Features.User.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.ApplicationUser? user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return new GetUserByIdResponse
            {
                Success = false,
                Message = "User not found"
            };
        }
        return new GetUserByIdResponse
        {
            DisplayName = user.DisplayName,
            Email = user.Email,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen,
            Success = true,
            Message = "Users fetch success"
        };
    }
}
