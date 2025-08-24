using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using InstantChat.Infrastructure.Data;

namespace InstantChat.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<ChatMessage>? _messages;
    private IRepository<GroupChat>? _groups;
    private IRepository<GroupChatParticipant>? _groupParticipants;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<ChatMessage> Messages =>
        _messages ??= new Repository<ChatMessage>(_context);

    public IRepository<GroupChat> Groups =>
        _groups ??= new Repository<GroupChat>(_context);

    public IRepository<GroupChatParticipant> GroupParticipants =>
        _groupParticipants ??= new Repository<GroupChatParticipant>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
