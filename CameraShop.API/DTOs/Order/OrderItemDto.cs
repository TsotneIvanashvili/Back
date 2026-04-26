namespace CameraShop.API.DTOs.Order;

public class OrderItemDto
{
    public int Id { get; set; }
    public int CameraId { get; set; }
    public string CameraModel { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}
