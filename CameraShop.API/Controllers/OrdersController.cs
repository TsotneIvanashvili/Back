using System.Security.Claims;
using CameraShop.API.DTOs.Order;
using CameraShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CameraShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service) => _service = service;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? throw new UnauthorizedAccessException("Missing user id claim."));

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMine() =>
        Ok(await _service.GetMyOrdersAsync(CurrentUserId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order is null) return NotFound();
        if (order.UserId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto dto)
    {
        var created = await _service.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
