using System.Linq.Expressions;
using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using InstantChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InstantChat.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        if (typeof(T) == typeof(ChatMessage))
        {
            return await _context.Set<ChatMessage>()
                .Include(m => m.Sender)
                .Where(predicate as Expression<Func<ChatMessage, bool>>)
                .Cast<T>()
                .ToListAsync();
        }

        if (typeof(T) == typeof(GroupChat))
        {
            return await _context.Set<GroupChat>()
                .Include(g => g.Participants)
                .ThenInclude(p => p.User)
                .Where(predicate as Expression<Func<GroupChat, bool>>)
                .Cast<T>()
                .ToListAsync();
        }

        return await _dbSet.Where(predicate).ToListAsync();
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        if (typeof(T) == typeof(ChatMessage))
        {
            return await _context.Set<ChatMessage>()
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(predicate as Expression<Func<ChatMessage, bool>>)
                as T;
        }

        if (typeof(T) == typeof(GroupChat))
        {
            return await _context.Set<GroupChat>()
                .Include(g => g.Participants)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(predicate as Expression<Func<GroupChat, bool>>)
                as T;
        }

        return await _dbSet.FirstOrDefaultAsync(predicate);
    }


    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
