using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CameraShop.API.DTOs.Cart;
using CameraShop.API.DTOs.Order;
using CameraShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CameraShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service) => _service = service;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? throw new UnauthorizedAccessException("Missing user id claim."));

    // GET /api/cart  — current user's cart
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetMine() =>
        Ok(await _service.GetMyCartAsync(CurrentUserId));

    // POST /api/cart/items  — add a camera to the cart
    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] CartItemAddDto dto) =>
        Ok(await _service.AddItemAsync(CurrentUserId, dto));

    // PUT /api/cart/items/{cameraId}  — update quantity of a camera in the cart
    [HttpPut("items/{cameraId:int}")]
    public async Task<ActionResult<CartDto>> UpdateItem(int cameraId, [FromBody] CartItemUpdateDto dto) =>
        Ok(await _service.UpdateItemAsync(CurrentUserId, cameraId, dto));

    // DELETE /api/cart/items/{cameraId}  — remove a single camera from the cart
    [HttpDelete("items/{cameraId:int}")]
    public async Task<ActionResult<CartDto>> RemoveItem(int cameraId) =>
        Ok(await _service.RemoveItemAsync(CurrentUserId, cameraId));

    // DELETE /api/cart  — empty the whole cart
    [HttpDelete]
    public async Task<ActionResult<CartDto>> Clear() =>
        Ok(await _service.ClearAsync(CurrentUserId));

    // POST /api/cart/checkout  — turn the cart into an Order, then empty it
    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout([FromBody] CheckoutDto dto) =>
        Ok(await _service.CheckoutAsync(CurrentUserId, dto.ShippingAddress));
}

public class CheckoutDto
{
    [Required, MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;
}
