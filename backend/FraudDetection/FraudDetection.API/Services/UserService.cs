using FraudDetection.API.Models;
using FraudDetection.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var user_list = await _context.Users.ToListAsync();
        return user_list;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var  found_user = await _context.Users.FirstOrDefaultAsync(t => t.UserId == id );
        
        return found_user;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}