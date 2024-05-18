/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using Microsoft.EntityFrameworkCore;
using MushroomPocket.Utils;

namespace MushroomPocket.Models;


[PrimaryKey("Id")]
public class Dungeon
{
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }
    public int EntryCost { get; set; }

    public DateTime Open { get; set; }
    public DateTime Close { get; set; }

    public virtual Profile Profile { get; set; } = null!;

    // STATIC STUFF
    private static readonly List<string> NAME_POOL = new List<string>() {
        ""
    };
    private static readonly List<string> DESC_POOL = new List<string>() {
        ""
    };


    public Dungeon()
    {
        Id = GenerateId();
        Difficulty = new Random().Next(1, 6);

        // Make time open scale with diff
        Open = DateTime.UtcNow;
        Close = Open.AddDays(Difficulty);

        // Make entry cost and rewards scale with diff
        EntryCost = 25 * Difficulty;

        Name = NAME_POOL[new Random().Next(0, NAME_POOL.Count)];
        Description = DESC_POOL[new Random().Next(0, DESC_POOL.Count)];
    }


    /// <summary>
    /// Generate id
    /// </summary>
    private static string GenerateId(MushroomContext db)
    {
        List<string> ids = db.Dungeons.Select((Dungeon d) => d.Id).ToList();
        return StringUtils.TinyId(ids);
    }
    private static string GenerateId()
    {
        using (MushroomContext db = new MushroomContext())
            return GenerateId(db);
    }


    /// <summary>
    /// Get difficulty string (I.E. S, A, B, ...)
    /// </summary>
    public string GetDifficulty() => GetDifficulty(Difficulty);
    public static string GetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 1: return "S";
            case 2: return "A";
            case 3: return "B";
            case 4: return "C";
            case 5: return "D";
            default: throw new Exception($"{difficulty} is not a valid difficulty");
        }
    }
}
