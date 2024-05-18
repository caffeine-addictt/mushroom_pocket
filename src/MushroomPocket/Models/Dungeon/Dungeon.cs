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

    /// <summary>Uncleared, Cleared</summary>
    public string Status { get; set; }

    public virtual Profile Profile { get; set; } = null!;

    // STATIC STUFF
    private static readonly List<string> NAME_POOL = new List<string>() {
        "Big Boo's Haunt",
        "Lethal Lava Land",
        "Pinna Park",
        "Sirena Beach",
        "Ghostly Galaxy",
        "Gusty Garden Galaxy",
        "Luncheon Kingdom",
        "Bowser's Kingdom",
        "Moon Kingdom",
    };
    private static readonly List<string> DESC_POOL = new List<string>() {
        "A vast battlefield with rolling hills and dangerous foes.",
        "A fortress with narrow ledges and moving platforms.",
        "An amusement park with exciting rides and attractions.",
        "A food-themed world with boiling lava and giant vegetables.",
        "A fortified castle with dangerous traps and powerful foes.",
        "A lunar landscape with low gravity and mysterious secrets.",
    };

    public string UnlockAsk => $"A mysterious force prevents you from entering the {GetDifficulty()} ranked dungeon... Bribe it with ${EntryCost}? [Y/N]";
    public string UnlockReject(float wallet) => $"A mysterious force scoffs at your measely ${wallet} while flinging you away. \"${EntryCost} or leave!\"";


    public Dungeon()
    {
        Id = GenerateId();
        Difficulty = new Random().Next(1, 6);

        Status = "Uncleared";

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
    public int GetDifficultyNum() => Difficulty;
    public static int GetDifficultyNum(string difficulty)
    {
        switch (difficulty)
        {
            case "S": return 1;
            case "A": return 2;
            case "B": return 3;
            case "C": return 4;
            case "D": return 5;
            default: throw new Exception($"{difficulty} is not a valid difficulty");
        }
    }
}
