using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace FraudDetection.API.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserService _UserService;

    public AuthService(ApplicationDbContext context,IUserService userService,IConfiguration configuration)
    {
        _context= context;
        _configuration = configuration;
        _UserService =userService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var exist_user = await _context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);

        if (exist_user != null)
        {
            return null;   // means user is already present!
        }

        string passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow, 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "user"
        };
        
        _context.Users.Add(user);
        
        await _context.SaveChangesAsync();

        string token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            return null;
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);

        if (!isPasswordValid)
        {
            return null;
        }
        string token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };
    }
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.Role,user.Role)
        };
   

        var key =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer:_configuration["Jwt:Issuer"], audience:_configuration["Jwt:Audience"], claims : claims , expires:DateTime.UtcNow.AddDays(1) , signingCredentials:credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}