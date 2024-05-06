using Microsoft.EntityFrameworkCore;
using MushroomPocket.Utils;

namespace MushroomPocket.Models;

public class MushroomContext : DbContext
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Character> Characters => Set<Character>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={SaveUtils.CurrentDbName}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>()
            .HasMany(t => t.Characters)
            .WithMany(c => c.Teams)
            .UsingEntity(j => j.ToTable("TeamCharacters"));
    }
}




