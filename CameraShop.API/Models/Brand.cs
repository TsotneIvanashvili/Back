namespace CameraShop.API.Models;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int FoundedYear { get; set; }

    // One-to-many: a Brand has many Cameras
    public ICollection<Camera> Cameras { get; set; } = new List<Camera>();
}
