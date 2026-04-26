namespace CameraShop.API.Models;
public class CameraCategory
{
    public int CameraId { get; set; }
    public Camera Camera { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
