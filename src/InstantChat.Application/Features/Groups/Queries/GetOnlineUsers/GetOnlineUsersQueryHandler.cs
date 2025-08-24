using AutoMapper;
using InstantChat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;

public class GetOnlineUsersQueryHandler : IRequestHandler<GetOnlineUsersQuery, GetOnlineUsersResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetOnlineUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<GetOnlineUsersResponse> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userManager.Users
                .Where(u => u.Id != request.CurrentUserId)
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new GetOnlineUsersResponse
            {
                Users = userDtos,
                Success = true,
                Message = "Users retrieved successfully"
            };
        }
        catch (Exception ex)
        {
            return new GetOnlineUsersResponse
            {
                Success = false,
                Message = $"Error retrieving users: {ex.Message}"
            };
        }
    }
}
