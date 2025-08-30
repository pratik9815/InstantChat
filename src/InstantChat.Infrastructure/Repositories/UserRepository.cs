using AutoMapper;
using InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;
using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using InstantChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InstantChat.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ApplicationUser>?> GetUserIdByNameAsync(string name)
    {
        if (name == null)
            return new List<ApplicationUser>();
        var user = await _context.Users
            .Where(u => u.DisplayName.Contains(name))
            .ToListAsync();
        
        if(!user.Any())
        {
            return new List<ApplicationUser>();
        }
        return user;
    }
    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        if(id == null)
            return new ApplicationUser();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);  
        return user;
    }
}
