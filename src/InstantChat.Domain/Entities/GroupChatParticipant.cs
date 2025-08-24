using InstantChat.Domain.Common;

namespace InstantChat.Domain.Entities;

public class GroupChatParticipant : BaseAuditableEntity
{
    public int GroupChatId { get; private set; }
    public string UserId { get; private set; } = string.Empty;

    // Navigation properties
    public virtual GroupChat GroupChat { get; private set; } = null!;
    public virtual ApplicationUser User { get; private set; } = null!;

    public GroupChatParticipant() { }

    public GroupChatParticipant(int groupChatId, string userId)
    {
        GroupChatId = groupChatId;
        UserId = userId;
        SetCreatedBy(userId);
    }

    public void Leave(string userId)
    {
        SoftDelete(userId);
    }

    public void Rejoin()
    {
        Restore();
    }
}