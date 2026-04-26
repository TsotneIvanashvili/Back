using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Cart;

public class CartItemUpdateDto
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}
