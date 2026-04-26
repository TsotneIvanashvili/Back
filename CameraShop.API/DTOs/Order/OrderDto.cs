namespace CameraShop.API.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}
