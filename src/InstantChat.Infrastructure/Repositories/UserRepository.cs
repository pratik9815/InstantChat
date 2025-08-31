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
        // Step 1: Get latest message timestamp per conversation
        var latestMessageTimes = await _context.ChatMessages
            .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
            .GroupBy(m => new { m.SenderId, m.ReceiverId })
            .Select(g => new
            {
                OtherUserId = g.Key.SenderId == currentUserId ? g.Key.ReceiverId : g.Key.SenderId,
                LastMessageTime = g.Max(m => m.Timestamp)
            })
            .OrderByDescending(x => x.LastMessageTime)
            .ToListAsync();

        if (!latestMessageTimes.Any())
            return new List<ApplicationUser>();

        // Step 2: Remove duplicate users, keep only the first (latest)
        var uniqueOtherUserIds = latestMessageTimes
            .GroupBy(x => x.OtherUserId)
            .Select(g => g.First())
            .Select(x => x.OtherUserId)
            .ToList();

        // Step 3: Fetch all users in one query
        var users = await _context.Users
            .Where(u => uniqueOtherUserIds.Contains(u.Id))
            .ToListAsync();

        // Step 4: Use dictionary for fast lookup
        var usersDict = users.ToDictionary(u => u.Id);

        // Step 5: Order users by latest message timestamp
        var orderedUsers = latestMessageTimes
            .Where(x => usersDict.ContainsKey(x.OtherUserId))
            .GroupBy(x => x.OtherUserId)          // remove duplicates again, just in case
            .Select(g => g.First())
            .Select(x => usersDict[x.OtherUserId])
            .ToList();

        return orderedUsers;
    }
}
