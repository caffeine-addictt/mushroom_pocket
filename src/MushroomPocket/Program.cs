namespace MushroomPocket
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 0)
      {
        Console.WriteLine("Usage: MushroomPocket");
        Environment.Exit(1);
      }

      // MushroomMaster criteria list for checking character transformation availability.
      List<MushroomMaster> mushroomMasters = new List<MushroomMaster>(){
        new MushroomMaster("Daisy", 2, "Peach"),
        new MushroomMaster("Wario", 3, "Mario"),
        new MushroomMaster("Waluigi", 1, "Luigi")
      };

      // Main event loop.
      while (true)
      {
        // Ask for action.
        Console.Write(String.Join("\n", [
          @"******************************",
          @"Welcome to Mushroom Pocket App",
          @"********************************",
          @"(1). Add Mushroom's character to my pocket",
          @"(2). List character(s) in my pocket",
          @"(3). Check if I can transform my characters",
          @"(4). Transform my character(s)",
          @"Please only enter [1, 2, 3, 4] or Q to quit: "
        ]));
        string? action = Console.ReadLine();

        // Panic if null
        if (action == null)
        {
          Console.WriteLine(@"PANIC: Action was null!");
          Environment.Exit(1);
        }

        switch (action.ToLower())
        {
          case "1":
            // TODO: Add character
            break;

          case "2":
            // TODO: List character
            break;

          case "3":
            // TODO: Check transformation
            break;

          case "4":
            // TODO: Transform character
            break;

          case "q":
            Console.WriteLine("Thanks for playing. Good bye!");
            Environment.Exit(0);
            break;

          default:
            Console.WriteLine(@"Invalid action. Please only enter [1, 2, 3, 4] or Q to quit.");
            break;
        }
      }
    }
  }
}
