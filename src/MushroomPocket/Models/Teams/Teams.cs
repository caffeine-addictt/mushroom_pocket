using Microsoft.EntityFrameworkCore;

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
        Id = Guid.NewGuid().ToString();
        Characters = new HashSet<Character>();
    }

    public Team(string name, string description, HashSet<Character> characters) : this(name, description)
    {
        Characters = characters;
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
