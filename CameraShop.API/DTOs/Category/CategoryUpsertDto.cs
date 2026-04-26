using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Category;

public class CategoryUpsertDto
{
    [Required, MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Description { get; set; } = string.Empty;
}
