using InstantChat.Domain.Common;

namespace InstantChat.Domain.Entities;

public class GroupChat : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string CreatedById { get; private set; } = string.Empty;

    // Navigation properties
    public virtual ApplicationUser CreatedBy { get; private set; } = null!;
    public virtual ICollection<GroupChatParticipant> Participants { get; private set; } = new List<GroupChatParticipant>();
    public virtual ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

    public GroupChat() { }

    public GroupChat(string name, string createdById, string? description = null)
    {
        Name = name;
        CreatedById = createdById;
        Description = description;
        SetCreatedBy(createdById);
    }

    public void UpdateDetails(string name, string? description, string updatedBy)
    {
        Name = name;
        Description = description;
        SetUpdatedBy(updatedBy);
    }

    public void Delete(string deletedBy)
    {
        SoftDelete(deletedBy);
    }
}