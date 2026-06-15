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
using System.Security.Cryptography;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserService _UserService;
    private readonly IAuditLogService _auditLogService;
    private readonly IEmailService _emailService;
    
    public AuthService(ApplicationDbContext context,IUserService userService,IConfiguration configuration , IAuditLogService auditLogService,IEmailService emailService)
    {
        _context= context;
        _configuration = configuration;
        _UserService =userService;
        _auditLogService = auditLogService;
        _emailService = emailService;
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
            PasswordHash = passwordhash,
            Role = "user",
        };
        
        _context.Users.Add(user);
        
        await _context.SaveChangesAsync();
        var emailbody =$"Hello {user.Name} \n Thankyou for registering .\n  Always ready to detect fraud transaction.\n Receive Live notifications and tracking of transactions. \n";
        
       _ = Task.Run(async () =>
    {
     try
        {
        await _emailService.SendEmailAsync(user.Email, "Created New User", emailbody);
        }
        catch (Exception e)
        {   
        Console.WriteLine(e + " failed to send email.");
        }
    });

        await _auditLogService.CreateLogAsync(user.UserId, "registered user","user",null,"created new user!");
        string Accesstoken = GenerateJwtToken(user);

        string refreshtoken = await CreateRefreshTokenAsync(user.UserId);
        return new AuthResponseDto
        {
            AccessToken = Accesstoken,
            RefreshToken = refreshtoken,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            await _auditLogService.CreateLogAsync(null, "Failed Login attempt", "user", null , $"Failed Attempt to login{dto.Email} ");
            return null;

        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);

        if (!isPasswordValid)
        {
            return null;
        }

        await _auditLogService.CreateLogAsync(user.UserId, "Successful Login attempt", "user", null , $"Logged in User' Email{dto.Email} and id {user.UserId}");

        string token = GenerateJwtToken(user);
        string refreshtoken = await CreateRefreshTokenAsync(user.UserId);
        return new AuthResponseDto
        {
            AccessToken = token,
            RefreshToken =refreshtoken,
            Email = user.Email,
            Role = user.Role
        };
    }
    
    public async Task<AuthResponseDto?> RefreshTokenAsync(string RefreshToken)
    {
        var StoredToken = await _context.RefreshTokens.Include(t => t.user).FirstOrDefaultAsync(r => r.Token == RefreshToken);

        if (StoredToken == null || StoredToken.IsRevoked || StoredToken.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        StoredToken.IsRevoked = true;
        var newRefreshToken = await CreateRefreshTokenAsync(StoredToken.UserId); 
        await _context.SaveChangesAsync();

        var newAccessToken = GenerateJwtToken(StoredToken.user);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Email = StoredToken.user.Email,
            Role = StoredToken.user.Role
        };
    }

    public async Task<bool> LogoutAsync(string refreshtoken)
    {
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshtoken);

        if (storedToken == null)
        {
            return false;
        }

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
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
        var token = new JwtSecurityToken(issuer:_configuration["Jwt:Issuer"], audience:_configuration["Jwt:Audience"], claims : claims , expires:DateTime.UtcNow.AddMinutes(15) , signingCredentials:credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    private async Task<string> CreateRefreshTokenAsync(int userId)
    {
        var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshtoken = new RefreshToken
        {
            UserId = userId,
            Token = tokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked =false
        };
        _context.RefreshTokens.Add(refreshtoken);
        await _context.SaveChangesAsync();

        return tokenValue;
    }


}