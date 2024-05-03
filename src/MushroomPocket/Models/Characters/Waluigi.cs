namespace MushroomPocket.Models;

public class Waluigi : Character
{
    public Waluigi(float hp, int exp) : base(hp, exp)
    {
        Name = "Waluigi";
        Skill = "Waluigi";
        EvolvedOnly = false;
    }
}
