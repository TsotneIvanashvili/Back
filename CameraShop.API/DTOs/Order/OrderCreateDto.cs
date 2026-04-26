using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Order;

public class OrderCreateDto
{
    [Required, MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<OrderItemCreateDto> Items { get; set; } = new();
}

public class OrderItemCreateDto
{
    [Required]
    public int CameraId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}
