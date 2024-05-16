/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using Microsoft.EntityFrameworkCore;
using MushroomPocket.Core;

namespace MushroomPocket.Models;


public class MushroomContext : DbContext
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<BattleLog> BattleLogs => Set<BattleLog>();

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

        modelBuilder.Entity<Profile>()
            .HasMany(p => p.Items)
            .WithOne(i => i.Profile)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Profile>()
            .HasMany(p => p.BattleLogs)
            .WithOne(b => b.Profile)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Short cut to get current profile
    /// </summary>
    public Profile GetProfile(
        bool includeTeams = false,
        bool includeCharacters = false,
        bool includeItems = false,
        bool includeBattleLogs = false
    )
    {
        DbSet<Profile> profiles = this.Profiles;
        IQueryable<Profile> query = profiles;

        query = includeTeams ? query.Include(p => p.Teams) : query;
        query = includeItems ? query.Include(p => p.Items) : query;
        query = includeCharacters ? query.Include(p => p.Characters) : query;
        query = includeBattleLogs ? query.Include(p => p.BattleLogs) : query;

        return query.Where(p => p.Id == Constants.CurrentProfileId).First()!;
    }

    /// <summary>
    /// Short cut to get Profiles
    /// </summary>
    public IQueryable<Profile> GetProfiles(bool includeCharacters = false, bool includeTeams = false, bool includeItems = false)
    {
        IQueryable<Profile> query = this.Profiles;
        query = includeTeams ? query.Include(p => p.Teams) : query;
        query = includeCharacters ? query.Include(p => p.Characters) : query;
        query = includeItems ? query.Include(p => p.Items) : query;
        return query;
    }

    /// <summary>
    /// Short cut to get Teams
    /// </summary>
    public IQueryable<Team> GetTeams(bool includeCharacters = false)
    {
        IQueryable<Team> query = this.Teams.Include(t => t.Profile).Where(t => t.Profile.Id == Constants.CurrentProfileId);
        query = includeCharacters ? query.Include(t => t.Characters) : query;
        return query;
    }

    /// <summary>
    /// Short cut to get Characters
    /// </summary>
    public IQueryable<Character> GetCharacters(bool includeTeams = false)
    {
        IQueryable<Character> query = this.Characters.Include(c => c.Profile).Where(c => c.Profile.Id == Constants.CurrentProfileId);
        query = includeTeams ? query.Include(c => c.Teams) : query;
        return query;
    }

    /// <summary>
    /// Short cut to get Items
    /// </summary>
    public IQueryable<Item> GetItems()
        => this.Items
            .Include(i => i.Profile)
            .Where(i => i.Profile.Id == Constants.CurrentProfileId);
    
    /// <summary>
    /// Short cut to get BattleLogs
    /// </summary>
    public IQueryable<BattleLog> GetBattleLogs()
        => this.BattleLogs
            .Include(b => b.Profile)
            .Where(b => b.Profile.Id == Constants.CurrentProfileId);
}




