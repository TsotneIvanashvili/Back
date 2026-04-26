namespace CameraShop.API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One-to-one: a User has one UserProfile
    public UserProfile? Profile { get; set; }

    // One-to-many: a User can have many Orders
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    // One-to-one: a User has one Cart
    public Cart? Cart { get; set; }
}
