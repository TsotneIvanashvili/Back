using AutoMapper;
using CameraShop.API.DTOs.Cart;
using CameraShop.API.DTOs.Order;
using CameraShop.API.Helpers;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using CameraShop.API.Services.Interfaces;

namespace CameraShop.API.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _carts;
    private readonly ICameraRepository _cameras;
    private readonly IOrderService _orders;
    private readonly IMapper _mapper;

    public CartService(
        ICartRepository carts,
        ICameraRepository cameras,
        IOrderService orders,
        IMapper mapper)
    {
        _carts = carts;
        _cameras = cameras;
        _orders = orders;
        _mapper = mapper;
    }

    public async Task<CartDto> GetMyCartAsync(int userId)
    {
        var cart = await _carts.GetOrCreateForUserAsync(userId);
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> AddItemAsync(int userId, CartItemAddDto dto)
    {
        var camera = await _cameras.GetByIdAsync(dto.CameraId)
            ?? throw new AppException($"Camera {dto.CameraId} not found.", 404);

        if (camera.StockQuantity < dto.Quantity)
            throw new AppException($"Not enough stock for {camera.Model}.", 400);

        var cart = await _carts.GetOrCreateForUserAsync(userId);
        var existing = await _carts.GetItemAsync(cart.Id, dto.CameraId);

        if (existing is null)
        {
            cart.Items.Add(new CartItem
            {
                CameraId = dto.CameraId,
                Quantity = dto.Quantity
            });
        }
        else
        {
            var newQty = existing.Quantity + dto.Quantity;
            if (camera.StockQuantity < newQty)
                throw new AppException($"Not enough stock for {camera.Model}.", 400);
            existing.Quantity = newQty;
        }

        await _carts.SaveAsync();
        return await GetMyCartAsync(userId);
    }

    public async Task<CartDto> UpdateItemAsync(int userId, int cameraId, CartItemUpdateDto dto)
    {
        var cart = await _carts.GetForUserWithItemsAsync(userId)
            ?? throw new AppException("Cart is empty.", 404);

        var item = await _carts.GetItemAsync(cart.Id, cameraId)
            ?? throw new AppException("Item not in cart.", 404);

        var camera = await _cameras.GetByIdAsync(cameraId)
            ?? throw new AppException("Camera not found.", 404);

        if (camera.StockQuantity < dto.Quantity)
            throw new AppException($"Not enough stock for {camera.Model}.", 400);

        item.Quantity = dto.Quantity;
        await _carts.SaveAsync();
        return await GetMyCartAsync(userId);
    }

    public async Task<CartDto> RemoveItemAsync(int userId, int cameraId)
    {
        var cart = await _carts.GetForUserWithItemsAsync(userId)
            ?? throw new AppException("Cart is empty.", 404);

        var item = await _carts.GetItemAsync(cart.Id, cameraId)
            ?? throw new AppException("Item not in cart.", 404);

        await _carts.RemoveItemAsync(item);
        await _carts.SaveAsync();
        return await GetMyCartAsync(userId);
    }

    public async Task<CartDto> ClearAsync(int userId)
    {
        var cart = await _carts.GetForUserWithItemsAsync(userId);
        if (cart is null) return await GetMyCartAsync(userId);

        await _carts.ClearAsync(cart);
        await _carts.SaveAsync();
        return await GetMyCartAsync(userId);
    }

    public async Task<OrderDto> CheckoutAsync(int userId, string shippingAddress)
    {
        var cart = await _carts.GetForUserWithItemsAsync(userId)
            ?? throw new AppException("Cart is empty.", 400);

        if (cart.Items.Count == 0)
            throw new AppException("Cart is empty.", 400);

        var orderDto = await _orders.CreateAsync(userId, new OrderCreateDto
        {
            ShippingAddress = shippingAddress,
            Items = cart.Items.Select(i => new OrderItemCreateDto
            {
                CameraId = i.CameraId,
                Quantity = i.Quantity
            }).ToList()
        });
        await _carts.ClearAsync(cart);
        await _carts.SaveAsync();

        return orderDto;
    }
}
