namespace MushroomPocket;


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

        List<Character> pocket = new List<Character>();

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

            switch (action == null ? action : action.ToLower())
            {
                case "1":
                    // Name
                    Console.Write("Enter Character's Name: ");
                    string? charName = Console.ReadLine();

                    if (!((string[])["Daisy", "Wario", "Waluigi"]).Contains(charName))
                    {
                        Console.WriteLine("\nInvalid character name. Please only enter ['Daisy', 'Wario', 'Waluigi'].");
                        break;
                    }

                    // HP
                    Console.Write("Enter Character's HP: ");
                    float charHP;
                    if (!float.TryParse(Console.ReadLine(), out charHP))
                    {
                        Console.WriteLine("\nInvalid HP. Please only enter a number.");
                        break;
                    }

                    // Exp
                    Console.Write("Enter Character's EXP: ");
                    int charEXP;
                    if (!int.TryParse(Console.ReadLine(), out charEXP))
                    {
                        Console.WriteLine("\nInvalid EXP. Please only enter an integer.");
                        break;
                    }

                    // Add char
                    // I'm too used to Golang lol
                    // if newChar, err := Character.from(charName, charHP, charEXP); err != nil {
                    //   // Error
                    // }
                    // :>
                    string? errOut;
                    Character? newChar = Character.from(charName!, charHP, charEXP, out errOut);
                    if (errOut != null)
                    {
                        Console.WriteLine("\n" + errOut);
                        break;
                    }

                    pocket.Add(newChar!);
                    Console.WriteLine($"{charName} has been added.");
                    break;

                case "2":
                    // Sort descending
                    pocket.Sort((Character c1, Character c2) => c2.Hp.CompareTo(c1.Hp));
                    foreach (Character c in pocket)
                    {
                        Console.WriteLine(String.Join("\n",
                            @"-----------------------",
                            $"Name: {c.Name}",
                            $"HP: {c.Hp}",
                            $"EXP: {c.Exp}",
                            $"Skill: {c.Skill}",
                            @"-----------------------"
                        ));
                    }
                    break;

                case "3":
                    // Check transformation
                    List<MushroomMaster> canEvoList = Character.canEvolve(pocket, mushroomMasters);
                    foreach (MushroomMaster m in canEvoList)
                    {
                        Console.WriteLine($"{m.Name} -> {m.TransformTo}");
                    }
                    break;

                case "4":
                    // TODO: Transform character
                    break;

                case "q" or null:
                    Console.WriteLine("Thanks for playing. Good bye!");
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("\nInvalid action. Please only enter [1, 2, 3, 4] or Q to quit.");
                    break;
            }

            // Insert newline
            Console.WriteLine();
        }
    }
}
