using AutoMapper;
using CameraShop.API.DTOs.Order;
using CameraShop.API.Helpers;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using CameraShop.API.Services.Interfaces;

namespace CameraShop.API.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICameraRepository _cameras;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orders, ICameraRepository cameras, IMapper mapper)
    {
        _orders = orders;
        _cameras = cameras;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<OrderDto>>(await _orders.GetAllWithDetailsAsync());

    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId) =>
        _mapper.Map<IEnumerable<OrderDto>>(await _orders.GetByUserAsync(userId));

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orders.GetByIdWithDetailsAsync(id);
        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateAsync(int userId, OrderCreateDto dto)
    {
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            Status = "Pending",
            OrderDate = DateTime.UtcNow
        };

        decimal total = 0m;
        foreach (var line in dto.Items)
        {
            var camera = await _cameras.GetByIdAsync(line.CameraId)
                ?? throw new AppException($"Camera {line.CameraId} not found.", 404);

            if (camera.StockQuantity < line.Quantity)
                throw new AppException($"Not enough stock for {camera.Model}.", 400);

            camera.StockQuantity -= line.Quantity;
            _cameras.Update(camera);

            var item = new OrderItem
            {
                CameraId = camera.Id,
                Quantity = line.Quantity,
                UnitPrice = camera.Price
            };
            order.Items.Add(item);
            total += item.UnitPrice * item.Quantity;
        }

        order.TotalAmount = total;
        await _orders.AddAsync(order);
        await _orders.SaveAsync();

        var refreshed = await _orders.GetByIdWithDetailsAsync(order.Id);
        return _mapper.Map<OrderDto>(refreshed!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return false;
        _orders.Remove(order);
        await _orders.SaveAsync();
        return true;
    }
}
