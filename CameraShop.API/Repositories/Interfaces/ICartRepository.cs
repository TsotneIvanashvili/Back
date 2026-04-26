using CameraShop.API.Models;

namespace CameraShop.API.Repositories.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart> GetOrCreateForUserAsync(int userId);
    Task<Cart?> GetForUserWithItemsAsync(int userId);
    Task<CartItem?> GetItemAsync(int cartId, int cameraId);
    Task RemoveItemAsync(CartItem item);
    Task ClearAsync(Cart cart);
}
