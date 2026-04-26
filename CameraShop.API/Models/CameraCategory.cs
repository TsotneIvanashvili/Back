namespace CameraShop.API.Models;

// Explicit join entity for the many-to-many between Camera and Category
public class CameraCategory
{
    public int CameraId { get; set; }
    public Camera Camera { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
