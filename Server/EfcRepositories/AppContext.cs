using Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories;
public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Absolute Path to one Shared DB File
        const string dbPath = @"C:\Users\waqar\IT-DNP1Y - Source\ForumApp\Server\WebApi\app.db";
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
