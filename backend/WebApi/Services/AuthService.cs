using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebApi.Exceptions;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Models.DTOs;
using WebApi.Repositories;

namespace WebApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Processing login for email: {Email}", request.Email);
        
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        if (_passwordHasher.VerifyHashedPassword(request.Email, user.PasswordHash, request.Password) != PasswordVerificationResult.Success)
        {
            throw new InvalidCredentialsException();
        }

        var token = GenerateJwtToken(user);
        
        return new AuthResponse
        {
            Token = token,
            User = user.ToDto()
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Processing registration for email: {Email}", request.Email);
        
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new EmailAlreadyExistsException(request.Email);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Email, request.Password);
        var user = request.ToEntity(passwordHash);
        
        var createdUser = await _userRepository.CreateAsync(user);
        var token = GenerateJwtToken(createdUser);
        
        return new AuthResponse
        {
            Token = token,
            User = createdUser.ToDto()
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        _logger.LogInformation("Validating token");
        
        var principal = ValidateJwtToken(token);
        return principal != null;
    }

    public ClaimsPrincipal? ValidateJwtToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            var issuer = jwtSettings["Issuer"] ?? "SampleBlog";
            var audience = jwtSettings["Audience"] ?? "SampleBlogUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        var issuer = jwtSettings["Issuer"] ?? "SampleBlog";
        var audience = jwtSettings["Audience"] ?? "SampleBlogUsers";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        _logger.LogInformation("Fetching user by ID: {UserId}", userId);
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        
        return user.ToDto();
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        _logger.LogInformation("Checking if email exists: {Email}", email);
        
        return await _userRepository.EmailExistsAsync(email);
    }
}