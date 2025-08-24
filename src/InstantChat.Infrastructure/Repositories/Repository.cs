using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
        // For ChatMessage, include Sender information
        //if (typeof(T) == typeof(Domain.Entities.ChatMessage))
        //{
        //    return await _context.Set<Domain.Entities.ChatMessage>()
        //        .Include(m => m.Sender)
        //        .Where(m => predicate.Compile()(m as T))
        //        .Cast<T>()
        //        .ToListAsync();
        //}
        if (typeof(T) == typeof(Domain.Entities.ChatMessage))
        {
            return await _context.Set<Domain.Entities.ChatMessage>()
                .Include(m => m.Sender)
                .Where(predicate as Expression<Func<Domain.Entities.ChatMessage, bool>>)
                .Cast<T>()
                .ToListAsync();
        }

        // For GroupChat, include Participants
        if (typeof(T) == typeof(Domain.Entities.GroupChat))
        {
            return await _context.Set<Domain.Entities.GroupChat>()
                .Include(g => g.Participants)
                .ThenInclude(p => p.User)
                .Where(g => predicate.Compile()(g as T))
                .Cast<T>()
                .ToListAsync();
        }

        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        // For ChatMessage, include Sender information
        if (typeof(T) == typeof(Domain.Entities.ChatMessage))
        {
            var message = await _context.Set<Domain.Entities.ChatMessage>()
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => predicate.Compile()(m as T));
            return message as T;
        }

        // For GroupChat, include Participants
        if (typeof(T) == typeof(Domain.Entities.GroupChat))
        {
            var group = await _context.Set<Domain.Entities.GroupChat>()
                .Include(g => g.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(g => predicate.Compile()(g as T));
            return group as T;
        }

        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
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
