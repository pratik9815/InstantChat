using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace InstantChat.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; private set; } = string.Empty;
    public DateTime LastSeen { get; private set; } = DateTime.UtcNow;
    public bool IsOnline { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<ChatMessage> SentMessages { get; private set; } = new List<ChatMessage>();
    public virtual ICollection<ChatMessage> ReceivedMessages { get; private set; } = new List<ChatMessage>();
    public virtual ICollection<GroupChatParticipant> GroupParticipations { get; private set; } = new List<GroupChatParticipant>();
    public virtual ICollection<GroupChat> CreatedGroups { get; private set; } = new List<GroupChat>();

    public ApplicationUser() { }

    public ApplicationUser(string email, string displayName)
    {
        Email = email;
        UserName = email;
        DisplayName = displayName;
    }

    public void UpdateOnlineStatus(bool isOnline)
    {
        IsOnline = isOnline;
        LastSeen = DateTime.UtcNow;
    }

    public void UpdateDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}
