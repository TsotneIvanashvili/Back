namespace CameraShop.API.Models;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public int CameraId { get; set; }
    public Camera Camera { get; set; } = null!;
}
