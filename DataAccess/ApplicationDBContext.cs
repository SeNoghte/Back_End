using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess;

public class ApplicationDBContext : DbContext
{
    protected readonly IConfiguration configuration;

    public DbSet<User> Users { get; set; }
    public DbSet<PendingVerification> PendingVerifications { get; set; }

    public ApplicationDBContext(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    }

}
