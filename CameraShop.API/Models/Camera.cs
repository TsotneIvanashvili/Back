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

    // Many-to-one back to Brand (the "many" side of one-to-many)
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    // Many-to-many: a Camera belongs to many Categories (e.g. Mirrorless + Vlogging)
    public ICollection<CameraCategory> CameraCategories { get; set; } = new List<CameraCategory>();

    // One-to-many back-ref: a Camera can appear in many OrderItems
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
