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

    {
      this.Name = name;
      this.NoToTransform = noToTransform;
      this.TransformTo = transformTo;
    }
  }
}
