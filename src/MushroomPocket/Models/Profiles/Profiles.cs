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
public class Profile
{
    public string Id { get; }
    public string Name { get; set; }
    public float Wallet { get; set; }

    public virtual HashSet<Team> Teams { get; set; } = null!;
    public virtual HashSet<Item> Items { get; set; } = null!;
    public virtual HashSet<Dungeon> Dungeons { get; set; } = null!;
    public virtual HashSet<Character> Characters { get; set; } = null!;
    public virtual HashSet<BattleLog> BattleLogs { get; set; } = null!;

    public Profile(string name)
    {
        Name = name;
        Wallet = 0.0f;
        Id = GenerateId();
    }

    /// <summary>
    /// Generate id
    /// </summary>
    private static string GenerateId(MushroomContext db)
    {
        List<string> ids = db.Teams.Select((Team t) => t.Id).ToList();
        return StringUtils.TinyId(ids);
    }
    private static string GenerateId()
    {
        using (MushroomContext db = new MushroomContext())
            return GenerateId(db);
    }
}
