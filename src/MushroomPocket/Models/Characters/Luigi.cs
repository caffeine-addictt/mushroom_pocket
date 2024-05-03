namespace MushroomPocket.Models;

public class Luigi : Character
{
    public Luigi(float hp, int exp) : base(hp, exp)
    {
        Name = "Luigi";
        EvolvedOnly = true;
        Skill = "Precision and Accuracy";
    }
}
