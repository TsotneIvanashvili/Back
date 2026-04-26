namespace CameraShop.API.DTOs.Cart;

public class CartItemDto
{
    public int Id { get; set; }
    public int CameraId { get; set; }
    public string CameraModel { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
