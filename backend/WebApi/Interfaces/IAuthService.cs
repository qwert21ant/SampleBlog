using System.Security.Claims;
using WebApi.Models.DTOs;

namespace WebApi.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<bool> IsEmailExistsAsync(string email);
    ClaimsPrincipal? ValidateJwtToken(string token);
}