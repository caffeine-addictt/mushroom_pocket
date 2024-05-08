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
public class Team
{
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public virtual HashSet<Character> Characters { get; set; }

    public Team(string name, string description)
    {
        Name = name;
        Description = description;
        Id = GenerateId();
        Characters = new HashSet<Character>();
    }

    public Team(string name, string description, HashSet<Character> characters) : this(name, description)
    {
        Characters = characters;
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

    /// <summary>
    /// Add a character to the team
    /// </summary>
    public void AddCharacter(Character character) => AddCharacterRange(new HashSet<Character> { character });
    public void AddCharacterRange(IEnumerable<Character> characters)
    {
        foreach (var character in characters)
        {
            Characters.Add(character);
        }
    }
}
