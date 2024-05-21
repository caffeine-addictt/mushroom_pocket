



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


/// <summary>
/// Record of each dungeon visit
/// </summary>
[PrimaryKey("Id")]
public class BattleLog
{
    public string Id { get; set; }

    public bool ClearedDungeon { get; set; }
    public string DungeonDifficulty { get; set; }

    public int CharactersUsed { get; set; }
    public int CharactersDead { get; set; }

    public int TotalDamageDealt { get; set; }
    public int TotalDamageTaken { get; set; }
    public DateTime Date { get; set; }

    public virtual Profile Profile { get; set; } = null!;


    public BattleLog(
        int charactersUsed,
        int charactersDead,
        bool clearedDungeon,
        string dungeonDifficulty,
        int totalDamageDealt,
        int totalDamageTaken
    )
    {
        Id = GenerateId();
        Date = DateTime.UtcNow;
        ClearedDungeon = clearedDungeon;
        TotalDamageDealt = totalDamageDealt;
        TotalDamageTaken = totalDamageTaken;
        DungeonDifficulty = dungeonDifficulty;
        CharactersUsed = charactersUsed;
        CharactersDead = charactersDead;
    }


    /// <summary>
    /// Generate id
    /// </summary>
    private static string GenerateId(MushroomContext db)
    {
        List<string> ids = db.BattleLogs.Select((BattleLog b) => b.Id).ToList();
        return StringUtils.TinyId(ids);
    }
    private static string GenerateId()
    {
        using (MushroomContext db = new MushroomContext())
            return GenerateId(db);
    }
}
