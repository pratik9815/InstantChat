using InstantChat.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstantChat.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    public DbSet<GroupChat> GroupChats { get; set; } = null!;
    public DbSet<GroupChatParticipant> GroupChatParticipants { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ChatMessage>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<GroupChat>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<GroupChatParticipant>().HasQueryFilter(c => !c.IsDeleted);

        // Configure ChatMessage
        builder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.SenderId).IsRequired();

            entity.HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.GroupChat)
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.GroupChatId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GroupChat
        builder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(g => g.CreatedBy)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure GroupChatParticipant
        builder.Entity<GroupChatParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(p => p.GroupChat)
                .WithMany(g => g.Participants)
                .HasForeignKey(p => p.GroupChatId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.User)
                .WithMany(u => u.GroupParticipations)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.GroupChatId, p.UserId }).IsUnique();
        });
    }
}
