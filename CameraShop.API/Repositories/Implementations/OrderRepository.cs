using CameraShop.API.Data;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Repositories.Implementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync() =>
        await _set
            .Include(o => o.User)
            .Include(o => o.Items).ThenInclude(i => i.Camera)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetByUserAsync(int userId) =>
        await _set
            .Include(o => o.User)
            .Include(o => o.Items).ThenInclude(i => i.Camera)
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<Order?> GetByIdWithDetailsAsync(int id) =>
        await _set
            .Include(o => o.User)
            .Include(o => o.Items).ThenInclude(i => i.Camera)
            .FirstOrDefaultAsync(o => o.Id == id);
}
