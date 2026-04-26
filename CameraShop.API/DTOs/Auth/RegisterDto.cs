using System.ComponentModel.DataAnnotations;

namespace CameraShop.API.DTOs.Auth;

public class RegisterDto
{
    [Required, MinLength(3), MaxLength(60)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(80)]
    public string City { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Country { get; set; } = string.Empty;
}
