using FraudDetection.API.Models;
using FraudDetection.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FraudDetection.API.DTOs;

namespace FraudDetection.API.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;
    private readonly IEmailService _emailService;

    public UserService(ApplicationDbContext context, IAuditLogService auditLogService,IEmailService emailService)
    {
        _context = context;
        _auditLogService = auditLogService;
        _emailService = emailService;
    }

    public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(UserFilterDto filterDto,int page, int pageSize)
    {
        var user_list = _context.Users.AsQueryable(); // get all user in list and doesnt block thread here. wait in background. .
        if (!string.IsNullOrEmpty(filterDto.Email))
        {
            user_list = user_list.Where(t => t.Email == filterDto.Email);
        }
        if (!string.IsNullOrEmpty(filterDto.Name))
        {
            user_list = user_list.Where(t => t.Name == filterDto.Name);
        }
        if (!string.IsNullOrEmpty(filterDto.Role))
        {
            user_list = user_list.Where(t => t.Role == filterDto.Role.ToLower());
        }
        if (filterDto.ToDate.HasValue)
        {
            user_list.Where(t => t.CreatedAt <= filterDto.ToDate.Value);
        }
        if (filterDto.FromData.HasValue)
        {
            user_list.Where(t => t.CreatedAt >= filterDto.FromData.Value);
        }
        if (filterDto.IsFlagged.HasValue)
        {
            user_list.Where( t => t.IsFlagged == filterDto.IsFlagged);
        }

        int list_length = await user_list.CountAsync();

        var items = await user_list.OrderByDescending( t =>t.UserId).Skip((page - 1)*pageSize).Take(pageSize).Select(t => new UserResponseDto
        {
            UserId = t.UserId,
            Name = t.Name,
            Email = t.Email,
            Role = t.Role,
            CreatedAt = t.CreatedAt,
            IsFlagged = t.IsFlagged
        }).ToListAsync();
        // adding user DTO
        return new PagedResult<UserResponseDto>
        {
            Items =items,
            Page = page,
            PageSize = pageSize,
            TotalRecords = list_length,
            TotalPages = (int)Math.Ceiling(list_length/(double)pageSize)
        };
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var  found_user = await _context.Users.FirstOrDefaultAsync(t => t.UserId == id );    // first or null
        if (found_user == null)
        {
            return null;
        }
        return new UserResponseDto
        {
            UserId = found_user.UserId,
            Name= found_user.Name,
            Email = found_user.Email, 
            Role = found_user.Role,
            CreatedAt = found_user.CreatedAt,
            IsFlagged = found_user.IsFlagged
        };
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto  dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow, 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role.ToLower()
        };
        
        _context.Users.Add(user);
        
        await _context.SaveChangesAsync();

        var emailbody =$"Hello {user.Name} \n Thankyou for registering .\n  Always ready to detect fraud transaction.\n Receive Live notifications and tracking of transactions. \n";
        try
        {
            _=Task.Run(() => _emailService.SendEmailAsync(user.Email,"Created New User",emailbody));
            
        }
        catch(Exception e)
        {
            Console.Write(e+" failed to send email.");
            await _auditLogService.CreateLogAsync(user.UserId, "failed to register","User", user.UserId,"failed to save user");
            // await _auditLogService.CreateLogAsync(user.UserId,"Failed email","User",user.UserId,"Transation Saved but failed to send email");

        }
        await _auditLogService.CreateLogAsync(user.UserId, "Created new user", "user",null , $"{dto.Name} { dto.Email} of new user");
         
        return new UserResponseDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            IsFlagged = user.IsFlagged
        };

        // return user;
    }
}