using CameraShop.API.Data;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Repositories.Implementations;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context) { }

    public async Task<Cart> GetOrCreateForUserAsync(int userId)
    {
        var cart = await _set
            .Include(c => c.Items).ThenInclude(i => i.Camera)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await _set.AddAsync(cart);
            await SaveAsync();
        }
        return cart;
    }

    public Task<Cart?> GetForUserWithItemsAsync(int userId) =>
        _set
            .Include(c => c.Items).ThenInclude(i => i.Camera)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    public Task<CartItem?> GetItemAsync(int cartId, int cameraId) =>
        _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cartId && i.CameraId == cameraId);

    public Task RemoveItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        return Task.CompletedTask;
    }

    public Task ClearAsync(Cart cart)
    {
        _context.CartItems.RemoveRange(cart.Items);
        return Task.CompletedTask;
    }
}
