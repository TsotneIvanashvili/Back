using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Camera;

public class CameraUpsertDto
{
    [Required, MaxLength(120)]
    public string Model { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, 10000)]
    public int StockQuantity { get; set; }

    [Range(1, 1000)]
    public int MegaPixels { get; set; }

    [MaxLength(60)]
    public string SensorType { get; set; } = string.Empty;

    [Url, MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public int BrandId { get; set; }

    public List<int> CategoryIds { get; set; } = new();
}
