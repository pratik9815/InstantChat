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
    public async Task<List<ApplicationUser>> GetLatestUsersByMessageAsync(string currentUserId)
    {
        try
        {
            // Get the latest message timestamp per conversation
            var latestMessageTimes = await _context.ChatMessages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .GroupBy(m => new { m.SenderId, m.ReceiverId })
                .Select(g => new
                {
                    OtherUserId = g.Key.SenderId == currentUserId ? g.Key.ReceiverId : g.Key.SenderId,
                    LastMessageTime = g.Max(m => m.Timestamp)
                })
                .OrderByDescending(x => x.LastMessageTime)
                .Distinct()
                .ToListAsync();

            if (!latestMessageTimes.Any())
                return new List<ApplicationUser>();

            // Fetch all users in one query
            var otherUserIds = latestMessageTimes.Select(x => x.OtherUserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .ToListAsync();

            // Order users by latest message timestamp
            var orderedUsers = latestMessageTimes
                .Select(x => users.FirstOrDefault(u => u.Id == x.OtherUserId))
                .Where(u => u != null)
                .ToList()!;

            return orderedUsers;
        }
        catch (Exception ex)
        {
            return new List<ApplicationUser>();
        }
        
    }


}
