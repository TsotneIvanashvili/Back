namespace CameraShop.API.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // One-to-one back-reference to User
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
