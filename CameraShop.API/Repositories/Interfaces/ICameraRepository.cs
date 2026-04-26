using CameraShop.API.Models;

namespace CameraShop.API.Repositories.Interfaces;

public interface ICameraRepository : IGenericRepository<Camera>
{
    Task<IEnumerable<Camera>> GetAllWithDetailsAsync();
    Task<Camera?> GetByIdWithDetailsAsync(int id);
    Task ReplaceCategoriesAsync(Camera camera, IEnumerable<int> categoryIds);
}
