using FraudDetection.API.DTOs;
using Microsoft.AspNetCore.Identity;
using FraudDetection.API.Models;

namespace FraudDetection.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher<User> hasher)
    {
        if (db.Users.Any())
            return;

        var adminDto = new CreateUserDto
        {
            Name = "Armaan",
            Email = "armaangill1616.com",
            Password = "AG",
            Role = "Admin"
        };

        var admin = new User
        {
            Name = adminDto.Name,
            Email = adminDto.Email,
            Role = adminDto.Role ?? "Admin"
        };

        admin.PasswordHash = hasher.HashPassword(admin, adminDto.Password);

        db.Users.Add(admin);

        await db.SaveChangesAsync();
    }
}