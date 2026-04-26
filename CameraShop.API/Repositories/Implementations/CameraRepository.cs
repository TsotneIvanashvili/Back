using CameraShop.API.Data;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Repositories.Implementations;

public class CameraRepository : GenericRepository<Camera>, ICameraRepository
{
    public CameraRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Camera>> GetAllWithDetailsAsync() =>
        await _set
            .Include(c => c.Brand)
            .Include(c => c.CameraCategories).ThenInclude(cc => cc.Category)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Camera?> GetByIdWithDetailsAsync(int id) =>
        await _set
            .Include(c => c.Brand)
            .Include(c => c.CameraCategories).ThenInclude(cc => cc.Category)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task ReplaceCategoriesAsync(Camera camera, IEnumerable<int> categoryIds)
    {
        var existing = await _context.CameraCategories
            .Where(cc => cc.CameraId == camera.Id)
            .ToListAsync();
        _context.CameraCategories.RemoveRange(existing);

        foreach (var cid in categoryIds.Distinct())
        {
            await _context.CameraCategories.AddAsync(new CameraCategory
            {
                CameraId = camera.Id,
                CategoryId = cid
            });
        }
    }
}
