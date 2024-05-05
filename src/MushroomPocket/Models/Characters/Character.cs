using Microsoft.EntityFrameworkCore;

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


public struct NameSimilarity
{
    public string Name;
    public float Confidence;

    public NameSimilarity(string name, float confidence)
    {
        Name = name;
        Confidence = confidence;
    }
}


[PrimaryKey("Id")]
public class Character
{
    public static readonly string[] ValidNames = [
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

    public List<Team> Teams { get; set; } = new List<Team>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Character"/> class.
    /// </summary>
    public Character(float hp, int exp)
    {
        Hp = hp;
        Exp = exp;
        Id = Guid.NewGuid().ToString();
    }
    public Character(float hp, int exp, string name, string skill, bool evolvedOnly) : this(hp, exp)
    {
        Name = name;
        Skill = skill;
        EvolvedOnly = evolvedOnly;
    }
    public Character(float hp, int exp, List<Team> teams) : this(hp, exp)
    {
        Teams = teams;
    }
    public Character(float hp, int exp, string name, string skill, bool evolvedOnly, List<Team> teams) : this(hp, exp, name, skill, evolvedOnly)
    {
        Teams = teams;
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
    /// Try find similar names
    /// </summary>
    public static bool TryParseName(string? name, List<string> allowList, out List<NameSimilarity> similarNames)
    {
        if (name == null)
        {
            similarNames = new List<NameSimilarity>();
            return false;
        }

        name = name.ToLower();

        string? validName;
        if (IsValidName(name, out validName))
        {
            similarNames = new List<NameSimilarity>() {
                new NameSimilarity(validName!, 1.0f)
            };
            return true;
        }

        // Fetch similar names
        similarNames = new List<NameSimilarity>();

        // This is extrememly expensive to run :"D
        foreach (string n in allowList)
        {
            // Weights should add up to 1.0f
            float startsWithWeight = 0.3f;
            float subStrWeight = 0.3f;
            float hasCharWeight = 0.4f;

            bool startsWith = false;
            bool subStr = false;

            float similarity = 0f;
            for (int i = 0; i < name.Length; i++)
            {
                string sub = name.Substring(name.Length - 1 - i);

                // StartsWith
                if (!startsWith && n.ToLower().StartsWith(sub))
                {
                    similarity += startsWithWeight * ((name.Length - i) / name.Length);
                    startsWith = true;
                }

                // SubStr
                if (!subStr && n.ToLower().Contains(sub))
                {
                    similarity += subStrWeight * ((name.Length - i) / name.Length);
                    subStr = true;
                }

                // hasChar
                if (n.ToLower().Contains(name[i]))
                {
                    similarity += hasCharWeight / name.Length;
                }
            }

            // Ignore if less than 30% confidence
            if (similarity >= 0.3f)
                similarNames.Add(new NameSimilarity(n, similarity));
        }

        if (similarNames.Count > 0)
        {
            similarNames.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates a new character from the given name, hp, and exp.
    /// </summary>
    public static Character? From(string name, float hp, int exp, out string? errOut, bool noEvolve = true)
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
        if (noEvolve) throw new ArgumentException($"{validName} can only be obtained by evolving!");

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
    public static int TimesEvolvable(int charCount, int noToTransform)
        => noToTransform == 0 ? 0 : (int)Math.Floor((decimal)(charCount / noToTransform));

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
            foreach (MushroomMaster evo in evoList)
            {
                int charCount = db.Characters.Where((Character c) => c.Name == evo.Name).Count();

                int evoCount;
                if (!CanBeEvolved(charCount, evo.NoToTransform, out evoCount)) continue;
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
            foreach (MushroomMaster m in evoList)
            {
                List<Character> charList = db.Characters.Where((Character c) => c.Name == m.Name).ToList();

                int evoCount;
                if (!CanBeEvolved(charList.Count, m.NoToTransform, out evoCount)) continue;

                // Update DB
                db.Characters.RemoveRange(charList.Take(m.NoToTransform * evoCount));
                db.AddRange(Enumerable.Repeat(Character.From(m.TransformTo, 100, 0, false), evoCount));
                db.SaveChanges();

                evolved.AddRange(Enumerable.Repeat(m, evoCount));
            }
        }

        return evolved;
    }
}
