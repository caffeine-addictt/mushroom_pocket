namespace MushroomPocket;


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


[Flags]
public enum CharacterNames
{
  Daisy,
  Luigi,
  Mario,
  Peach,
  Waluigi,
  Wario,
}

public abstract class Character
{
  public int Exp { get; set; }
  public float Hp { get; set; }
  public abstract string Name { get; }
  public abstract string Skill { get; }
  public abstract bool EvolvedOnly { get; }

  public Character(float hp, int exp)
  {
    this.Hp = hp;
    this.Exp = exp;
  }

  /// <summary>
  /// Validate name
  /// </summary>
  public static bool isValidName(string? name, out CharacterNames charName) => Enum.TryParse<CharacterNames>(name, out charName);

  /// <summary>
  /// Get a character from its name
  ///
  /// Throws if name is an invalid character
  /// </summary>
  public static Character? from(string name, float hp, int exp, out string? errOut, bool noEvolve = true)
  {
    CharacterNames characterName;
    if (!Character.isValidName(name, out characterName))
    {
      errOut = $"Character name {name} is invalid!";
      return null;
    }

    return _from(characterName, hp, exp, out errOut, noEvolve);
  }

  /// <summary>
  /// Get a character from its enum
  /// </summary>
  public static Character? from(CharacterNames characterName, float hp, int exp, out string? errOut, bool noEvolve = true)
  {
    return _from(characterName, hp, exp, out errOut, noEvolve);
  }

  private static Character? _from(CharacterNames characterName, float hp, int exp, out string? errOut, bool noEvolve = true)
  {
    Character c;
    switch (characterName)
    {
      case CharacterNames.Daisy:
        c = new Daisy(hp, exp);
        break;
      case CharacterNames.Luigi:
        c = new Luigi(hp, exp);
        break;
      case CharacterNames.Mario:
        c = new Mario(hp, exp);
        break;
      case CharacterNames.Peach:
        c = new Peach(hp, exp);
        break;
      case CharacterNames.Waluigi:
        c = new Waluigi(hp, exp);
        break;
      case CharacterNames.Wario:
        c = new Wario(hp, exp);
        break;
      default:
        errOut = "Character was not found!";
        return null;
    }

    if (noEvolve && c.EvolvedOnly)
    {
      errOut = $"Character name {c.Name} can only be obtained by evolving!";
      return null;
    }

    errOut = null;
    return c;
  }
}

public class Daisy(float hp, int exp) : Character(hp, exp)
{
  public override string Name { get; } = "Daisy";
  public override bool EvolvedOnly { get; } = false;
  public override string Skill { get; } = "Leadership";
}

public class Waluigi(float hp, int exp) : Character(hp, exp)
{
  public override string Name { get; } = "Waluigi";
  public override bool EvolvedOnly { get; } = false;
  public override string Skill { get; } = "Agility";
}

public class Wario(float hp, int exp) : Character(hp, exp)
{
  public override string Name { get; } = "Wario";
  public override bool EvolvedOnly { get; } = false;
  public override string Skill { get; } = "Strength";
}

public class Luigi(float hp, int exp) : Character(hp, exp)
{
  public override string Name { get; } = "Luigi";
  public override bool EvolvedOnly { get; } = true;
  public override string Skill { get; } = "Precision and Accuracy";
}

public class Peach(float hp, int exp) : Character(hp, exp)
{
  public override string Name { get; } = "Peach";
  public override bool EvolvedOnly { get; } = true;
  public override string Skill { get; } = "Magic Abilities";
}

public class Mario(float hp, int exp) : Character(hp, exp)
{
  public override string Name { get; } = "Mario";
  public override bool EvolvedOnly { get; } = true;
  public override string Skill { get; } = "Combat Skills";
}
