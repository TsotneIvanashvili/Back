namespace CameraShop.API.DTOs.Camera;

public class CameraDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int MegaPixels { get; set; }
    public string SensorType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;

    public List<string> Categories { get; set; } = new();
}
