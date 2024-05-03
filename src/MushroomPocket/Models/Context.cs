using Microsoft.EntityFrameworkCore;

namespace MushroomPocket.Models;

public class MushroomContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(@"Data Source=mushroom.db");
    }
}




