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

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetMine() =>
        Ok(await _service.GetMyCartAsync(CurrentUserId));

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] CartItemAddDto dto) =>
        Ok(await _service.AddItemAsync(CurrentUserId, dto));

    [HttpPut("items/{cameraId:int}")]
    public async Task<ActionResult<CartDto>> UpdateItem(int cameraId, [FromBody] CartItemUpdateDto dto) =>
        Ok(await _service.UpdateItemAsync(CurrentUserId, cameraId, dto));

    [HttpDelete("items/{cameraId:int}")]
    public async Task<ActionResult<CartDto>> RemoveItem(int cameraId) =>
        Ok(await _service.RemoveItemAsync(CurrentUserId, cameraId));

    [HttpDelete]
    public async Task<ActionResult<CartDto>> Clear() =>
        Ok(await _service.ClearAsync(CurrentUserId));

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout([FromBody] CheckoutDto dto) =>
        Ok(await _service.CheckoutAsync(CurrentUserId, dto.ShippingAddress));
}

public class CheckoutDto
{
    [Required, MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;
}
