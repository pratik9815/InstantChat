using InstantChat.Domain.Entities;

public interface IUserRepository
{
    Task<List<ApplicationUser>?> GetUserIdByNameAsync(string name);
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<List<ApplicationUser>> GetLatestUsersByMessageAsync(string currentUserId);
}
