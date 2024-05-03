namespace MushroomPocket.Models;

public class Wario : Character
{
    public Wario(float hp, int exp) : base(hp, exp)
    {
        Name = "Wario";
        Skill = "Strength";
        EvolvedOnly = false;
    }
}
