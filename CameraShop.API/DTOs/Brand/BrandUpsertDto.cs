using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Brand;

public class BrandUpsertDto
{
    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Country { get; set; } = string.Empty;

    [Range(1800, 2100)]
    public int FoundedYear { get; set; }
}
