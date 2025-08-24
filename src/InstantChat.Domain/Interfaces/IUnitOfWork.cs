using InstantChat.Domain.Entities;

namespace InstantChat.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<ChatMessage> Messages { get; }
    IRepository<GroupChat> Groups { get; }
    IRepository<GroupChatParticipant> GroupParticipants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
