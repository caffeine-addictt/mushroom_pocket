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
    public string Name { get; set; } = "";
    public string Skill { get; set; } = "";
    public bool EvolvedOnly { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Character"/> class.
    /// </summary>
    public Character(float hp, int exp)
    {
        Hp = hp;
        Exp = exp;
        Id = Guid.NewGuid().ToString();
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
    /// Check if character can evolve
    /// </summary>
    public static List<MushroomMaster> CanEvolve(List<MushroomMaster> evoList)
    {
        List<MushroomMaster> canEvolve = new List<MushroomMaster>();
        using (MushroomContext db = new MushroomContext())
        {
            foreach (MushroomMaster evo in evoList)
            {
                // NoToTransform is 0 if the character can never be obtained
                if (evo.NoToTransform == 0) continue;

                int charCount = db.Characters.Where((Character c) => c.Name == evo.Name).Count();

                // Count No. of times the character can be evolved
                int evoCount = (int)Math.Floor((decimal)(charCount / evo.NoToTransform));
                if (evoCount == 0) continue;

                canEvolve.AddRange(Enumerable.Repeat(evo, evoCount));
            }
        }

        return canEvolve;
    }
}
