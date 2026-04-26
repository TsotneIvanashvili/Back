namespace CameraShop.API.Models;

public class Cart
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One-to-one back to User
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // One-to-many: a Cart has many CartItems
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
