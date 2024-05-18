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


/// Flags
/// <summary>
/// Allow using an integer to store the flags
///
/// Example:
///     - Including Teams mean to pass the IncludeFlags.Teams or 0b00000001
///     - Including Teams or Dungeons mean to pass IncludeFlags.Teams | IncludeFlags.Dungeons or 0b00010001
/// </summary>
public enum IncludeFlags : byte
{
    None = 0,
    Teams = 1,
    Characters = 2,
    Items = 4,
    BattleLogs = 8,
    Dungeons = 16,

    /// <summary>Includes Character.Teams</summary>
    CharacterTeams = 32,

    /// <summary>Includes Team.Characters</summary>
    TeamCharacters = 64,

    CharactersAndItems = Characters | Items,
    TeamsAndDungeons = Teams | Dungeons,
}


// Context
public class MushroomContext : DbContext
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Dungeon> Dungeons => Set<Dungeon>();
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

        modelBuilder.Entity<Profile>()
            .HasMany(p => p.Dungeons)
            .WithOne(d => d.Profile)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Short cut to get current profile
    ///
    /// Includes are 2 deep max
    /// </summary>
    public Profile GetProfile(IncludeFlags flags = IncludeFlags.None)
    {
        IQueryable<Profile> query = this.Profiles;

        // Handle the non multi-linked includes
        if (flags.HasFlag(IncludeFlags.BattleLogs))
            query = query.Include(p => p.BattleLogs);

        if (flags.HasFlag(IncludeFlags.Dungeons))
            query = query.Include(p => p.Dungeons);

        if (flags.HasFlag(IncludeFlags.Items))
            query = query.Include(p => p.Items);

        // Handle multi-linked includes
        if (flags.HasFlag(IncludeFlags.Teams))
            query = flags.HasFlag(IncludeFlags.Characters)
                ? query.Include(p => p.Teams).ThenInclude(t => t.Characters)
                : query.Include(p => p.Teams);

        if (flags.HasFlag(IncludeFlags.Characters))
            query = flags.HasFlag(IncludeFlags.Teams)
                ? query.Include(p => p.Characters).ThenInclude(c => c.Teams)
                : query.Include(p => p.Characters);

        return query.First(p => p.Id == Constants.CurrentProfileId);
    }

    /// <summary>
    /// Short cut to get Profiles
    ///
    /// Includes are 2 deep max
    /// </summary>
    public IQueryable<Profile> GetProfiles(IncludeFlags flags = IncludeFlags.None)
    {
        IQueryable<Profile> query = this.Profiles;

        // Handle the non multi-linked includes
        if (flags.HasFlag(IncludeFlags.BattleLogs))
            query = query.Include(p => p.BattleLogs);

        if (flags.HasFlag(IncludeFlags.Dungeons))
            query = query.Include(p => p.Dungeons);

        if (flags.HasFlag(IncludeFlags.Items))
            query = query.Include(p => p.Items);

        // Handle multi-linked includes
        if (flags.HasFlag(IncludeFlags.Teams))
            query = flags.HasFlag(IncludeFlags.TeamCharacters)
                ? query.Include(p => p.Teams).ThenInclude(t => t.Characters)
                : query.Include(p => p.Teams);

        if (flags.HasFlag(IncludeFlags.Characters))
            query = flags.HasFlag(IncludeFlags.CharacterTeams)
                ? query.Include(p => p.Characters).ThenInclude(c => c.Teams)
                : query.Include(p => p.Characters);

        return query;
    }

    /// <summary>
    /// Short cut to get Teams
    ///
    /// Converts IncludeFlags.Characters to IncludeFlags.TeamCharacters
    /// Force flags to only be able to contain IncludeFlags.Characters then ensure IncludeFlags.Teams is set.
    /// </summary>
    public IQueryable<Team> GetTeams(IncludeFlags flags = IncludeFlags.None)
        => GetProfile(
            IncludeFlags.Teams
            | (
                (flags.HasFlag(IncludeFlags.Characters) ? IncludeFlags.TeamCharacters : flags)
                & IncludeFlags.TeamCharacters
            )).Teams.AsQueryable();

    /// <summary>
    /// Short cut to get Characters
    ///
    /// Converts IncludeFlags.Teams to IncludeFlags.CharacterTeams
    /// Force flags to only be able to contain IncludeFlags.CharacterTeams then ensure IncludeFlags.Characters is set.
    /// </summary>
    public IQueryable<Character> GetCharacters(IncludeFlags flags = IncludeFlags.None)
        => GetProfile(
            IncludeFlags.Characters
            | (
                (flags.HasFlag(IncludeFlags.Teams) ? IncludeFlags.CharacterTeams : flags)
                & IncludeFlags.CharacterTeams
            )).Characters.AsQueryable();

    /// <summary>
    /// Short cut to get Items
    /// </summary>
    public IQueryable<Item> GetItems()
        => GetProfile(IncludeFlags.Items).Items.AsQueryable();

    /// <summary>
    /// Short cut to get BattleLogs
    /// </summary>
    public IQueryable<BattleLog> GetBattleLogs()
        => GetProfile(IncludeFlags.BattleLogs).BattleLogs.AsQueryable();
}




