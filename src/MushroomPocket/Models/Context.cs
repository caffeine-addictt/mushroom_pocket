/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using Microsoft.EntityFrameworkCore;

namespace MushroomPocket.Models;


public class MushroomContext : DbContext
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Character> Characters => Set<Character>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source=mushroom.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>()
            .HasMany(t => t.Characters)
            .WithMany(c => c.Teams)
            .UsingEntity(j => j.ToTable("TeamCharacters"));

        modelBuilder.Entity<Profile>()
            .HasMany(p => p.Characters)
            .WithOne(c => c.Profile)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Profile>()
            .HasMany(p => p.Teams)
            .WithOne(t => t.Profile)
            .OnDelete(DeleteBehavior.Cascade);
    }
}




