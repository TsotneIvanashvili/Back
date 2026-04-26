using CameraShop.API.Models;

namespace CameraShop.API.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);
}
