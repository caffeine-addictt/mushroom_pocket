namespace MushroomPocket.Models;

public class Mario : Character
{
    public Mario(float hp, int exp) : base(hp, exp)
    {
        Name = "Mario";
        EvolvedOnly = true;
        Skill = "Combat Skills";
    }
}
