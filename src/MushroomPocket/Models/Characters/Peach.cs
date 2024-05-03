namespace MushroomPocket.Models;

public class Peach : Character
{
    public Peach(float hp, int exp) : base(hp, exp)
    {
        Name = "Peach";
        EvolvedOnly = true;
        Skill = "magic abilities";
    }
}
