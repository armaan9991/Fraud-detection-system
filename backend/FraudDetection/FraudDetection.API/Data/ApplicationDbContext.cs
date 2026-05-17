using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Data;

public class ApplicationDbContext:DbContext                        // our file ApplicationDbContext is inherting DB context from EF means now we can handle our database
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)      //DbContextOptions are configuration settings like connection strings
    {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FraudAlert> FraudAlerts => Set<FraudAlert>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)  // allows database configuration.
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
    }
    
}