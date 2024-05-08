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


public class MushroomMaster
{
    public string Name { get; set; }
    public int NoToTransform { get; set; }
    public string TransformTo { get; set; }

    public MushroomMaster(string name, int noToTransform, string transformTo)
    {
        this.Name = name;
        this.NoToTransform = noToTransform;
        this.TransformTo = transformTo;
    }
}


[PrimaryKey("Id")]
public class Character
{
    public static readonly string[] ValidNames =
    [
        "Daisy",
        "Luigi",
        "Mario",
        "Peach",
        "Waluigi",
        "Wario",
    ];

    public string Id { get; set; }

    public int Exp { get; set; }
    public float Hp { get; set; }
    public string Name { get; set; } = null!;
    public string Skill { get; set; } = null!;
    public bool EvolvedOnly { get; set; } = true;


    public virtual Profile Profile { get; set; } = null!;
    public virtual HashSet<Team> Teams { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Character"/> class.
    /// </summary>
    public Character(float hp, int exp)
    {
        Hp = hp;
        Exp = exp;
        Id = GenerateId();
    }

    public Character(float hp, int exp, string name, string skill, bool evolvedOnly)
        : this(hp, exp)
    {
        Name = name;
        Skill = skill;
        EvolvedOnly = evolvedOnly;
    }

    public Character(float hp, int exp, HashSet<Team> teams)
        : this(hp, exp)
    {
        Teams = teams;
    }

    public Character(
        float hp,
        int exp,
        string name,
        string skill,
        bool evolvedOnly,
        HashSet<Team> teams
    )
        : this(hp, exp, name, skill, evolvedOnly)
    {
        Teams = teams;
    }

    /// <summary>
    /// Generate id
    /// </summary>
    private static string GenerateId(MushroomContext db)
    {
        List<string> ids = db.Characters.Select((Character c) => c.Id).ToList();
        return StringUtils.TinyId(ids);
    }
    private static string GenerateId()
    {
        using (MushroomContext db = new MushroomContext())
            return GenerateId(db);
    }

    /// <summary>
    /// See if a string is a valid character name.
    /// Strings are converted to TitleCase and compared to ValidNames only if validName is specified.
    /// </summary>
    public static bool IsValidName(string name) => ValidNames.Contains(name);

    public static bool IsValidName(string name, out string? validName)
    {
        name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
        if (ValidNames.Contains(name))
        {
            validName = name;
            return true;
        }

        validName = null;
        return false;
    }

    /// <summary>
    /// Creates a new character from the given name, hp, and exp.
    /// </summary>
    public static Character? From(
        string name,
        float hp,
        int exp,
        out string? errOut,
        bool noEvolve = true
    )
    {
        try
        {
            Character c = From(name, hp, exp, noEvolve);
            errOut = null;
            return c;
        }
        catch (Exception e)
        {
            errOut = e.Message;
            return null;
        }
    }

    public static Character From(string name, float hp, int exp, bool noEvolve = true)
    {
        string? validName;
        if (!IsValidName(name, out validName))
        {
            throw new ArgumentException($"{name} is an invalid character name!");
        }

        // Handle non-evolved characters
        switch (validName)
        {
            case "Daisy":
                return new Daisy(hp, exp);
            case "Waluigi":
                return new Waluigi(hp, exp);
            case "Wario":
                return new Wario(hp, exp);
        }

        // Handle evolved characters
        if (noEvolve)
            throw new ArgumentException($"{validName} can only be obtained by evolving!");

        switch (validName)
        {
            case "Peach":
                return new Peach(hp, exp);
            case "Luigi":
                return new Luigi(hp, exp);
            case "Mario":
                return new Mario(hp, exp);
        }

        throw new ArgumentException($"{validName} is an unknown character!");
    }

    /// <summary>
    /// See number of times character can be evolved
    ///
    /// </summary>
    public static int TimesEvolvable(int charCount, int noToTransform) =>
        noToTransform == 0 ? 0 : (int)Math.Floor((decimal)(charCount / noToTransform));

    /// <summary>
    /// See if character can be evolved
    /// noToTransform is 0 if the character can never be obtained
    /// </summary>
    public static bool CanBeEvolved(int charCount, int noToTransform, out int evoCount)
    {
        evoCount = TimesEvolvable(charCount, noToTransform);
        return evoCount != 0;
    }

    /// <summary>
    /// Check if character can evolve
    /// </summary>
    public static List<MushroomMaster> CanEvolve(List<MushroomMaster> evoList)
    {
        List<MushroomMaster> canEvolve = new List<MushroomMaster>();
        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(false, true);
            foreach (MushroomMaster evo in evoList)
            {
                int charCount = profile.Characters.Where((Character c) => c.Name == evo.Name).Count();

                int evoCount;
                if (!CanBeEvolved(charCount, evo.NoToTransform, out evoCount))
                    continue;
                canEvolve.AddRange(Enumerable.Repeat(evo, evoCount));
            }
        }

        return canEvolve;
    }

    /// <summary>
    /// Evolve Characters
    /// </summary>
    public static List<MushroomMaster> Evolve(List<MushroomMaster> evoList)
    {
        List<MushroomMaster> evolved = new List<MushroomMaster>();
        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(true, true);
            foreach (MushroomMaster m in evoList)
            {
                List<Character> charList = profile
                    .Characters.Where((Character c) => c.Name == m.Name)
                    .ToList();

                int evoCount;
                if (!CanBeEvolved(charList.Count, m.NoToTransform, out evoCount))
                    continue;

                // Update DB
                profile.Characters.ExceptWith(charList.Take(m.NoToTransform * evoCount));
                profile.Characters.UnionWith(
                    Enumerable.Repeat(Character.From(m.TransformTo, 100, 0, false), evoCount)
                );
                db.SaveChanges();

                evolved.AddRange(Enumerable.Repeat(m, evoCount));
            }
        }

        return evolved;
    }
}
