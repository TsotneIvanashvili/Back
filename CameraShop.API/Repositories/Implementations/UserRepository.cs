using CameraShop.API.Data;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail) =>
        await _set
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

    public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email) =>
        await _set.AnyAsync(u => u.Username == username || u.Email == email);
}
