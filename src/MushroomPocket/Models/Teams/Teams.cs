using Microsoft.EntityFrameworkCore;

namespace MushroomPocket.Models;


[PrimaryKey("Id")]
public class Team
{
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public List<Character> Characters { get; set; }

    public Team(string name, string description)
    {
        Name = name;
        Description = description;
        Id = Guid.NewGuid().ToString();
        Characters = new List<Character>();
    }

    public Team(string name, string description, List<Character> characters) : this(name, description)
    {
        Characters = characters;
    }

    /// <summary>
    /// Add a character to the team
    /// </summary>
    public void AddCharacter(Character character) => AddCharacterRange(new List<Character> { character });
    public void AddCharacterRange(List<Character> characters)
    {
        Characters.AddRange(characters);
    }
}
