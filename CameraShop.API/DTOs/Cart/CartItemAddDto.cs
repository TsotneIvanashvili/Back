using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Cart;

public class CartItemAddDto
{
    [Required]
    public int CameraId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}
