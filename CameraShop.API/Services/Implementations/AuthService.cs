using CameraShop.API.DTOs.Auth;
using CameraShop.API.Helpers;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using CameraShop.API.Services.Interfaces;

namespace CameraShop.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly JwtTokenGenerator _jwt;

    public AuthService(IUserRepository users, JwtTokenGenerator jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _users.ExistsByUsernameOrEmailAsync(dto.Username, dto.Email))
            throw new AppException("Username or email is already in use.", 409);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer",
            Profile = new UserProfile
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country
            }
        };

        await _users.AddAsync(user);
        await _users.SaveAsync();

        var (token, expires) = _jwt.Generate(user);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expires,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByUsernameOrEmailAsync(dto.UsernameOrEmail)
            ?? throw new AppException("Invalid credentials.", 401);

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new AppException("Invalid credentials.", 401);

        var (token, expires) = _jwt.Generate(user);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expires,
            Username = user.Username,
            Role = user.Role
        };
    }
}
