using CameraShop.API.DTOs.Order;

namespace CameraShop.API.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(int userId, OrderCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
