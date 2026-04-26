using CameraShop.API.DTOs.Cart;
using CameraShop.API.DTOs.Order;

namespace CameraShop.API.Services.Interfaces;

public interface ICartService
{
    Task<CartDto> GetMyCartAsync(int userId);
    Task<CartDto> AddItemAsync(int userId, CartItemAddDto dto);
    Task<CartDto> UpdateItemAsync(int userId, int cameraId, CartItemUpdateDto dto);
    Task<CartDto> RemoveItemAsync(int userId, int cameraId);
    Task<CartDto> ClearAsync(int userId);
    Task<OrderDto> CheckoutAsync(int userId, string shippingAddress);
}
