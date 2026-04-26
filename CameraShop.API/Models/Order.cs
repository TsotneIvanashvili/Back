namespace CameraShop.API.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;

    // Many-to-one back to User (the "many" side of one-to-many)
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // One-to-many: an Order has many OrderItems
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
