namespace MushroomPocket.Models;


public class Daisy : Character
{
    public Daisy(float hp, int exp) : base(hp, exp)
    {
        Name = "Daisy";
        EvolvedOnly = false;
        Skill = "Leadership";
    }
}
