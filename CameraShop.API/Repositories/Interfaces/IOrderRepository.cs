using CameraShop.API.Models;

namespace CameraShop.API.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();
    Task<IEnumerable<Order>> GetByUserAsync(int userId);
    Task<Order?> GetByIdWithDetailsAsync(int id);
}
