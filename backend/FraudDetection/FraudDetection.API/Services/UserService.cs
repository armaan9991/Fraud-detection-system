using FraudDetection.API.Models;
using FraudDetection.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FraudDetection.API.DTOs;

namespace FraudDetection.API.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int page, int pageSize,UserFilterDto filterDto)
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
            user_list = user_list.Where(t => t.Role == filterDto.Role);
        }
        if (filterDto.ToDate.HasValue)
        {
            user_list.Where(t => t.CreatedAt <= filterDto.ToDate.Value);
        }
        if (filterDto.FromData.HasValue)
        {
            user_list.Where(t => t.CreatedAt >= filterDto.FromData.Value);
        }

        int list_length = await user_list.CountAsync();

        var items = await user_list.OrderByDescending( t =>t.UserId).Skip((page - 1)*pageSize).Take(pageSize).Select(t => new UserResponseDto
        {
            UserId = t.UserId,
            Name = t.Name,
            Email = t.Email,
            Role = t.Role,
            CreatedAt = t.CreatedAt
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

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var  found_user = await _context.Users.FirstOrDefaultAsync(t => t.UserId == id );    // first or null
        
        return found_user;
    }

    public async Task<User> CreateUserAsync(CreateUserDto  dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow, 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };
        
        _context.Users.Add(user);
        
        await _context.SaveChangesAsync();
        
        return user;
    }
}