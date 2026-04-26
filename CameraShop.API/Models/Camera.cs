namespace CameraShop.API.Models;

public class Camera
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int MegaPixels { get; set; }
    public string SensorType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
    public ICollection<CameraCategory> CameraCategories { get; set; } = new List<CameraCategory>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
