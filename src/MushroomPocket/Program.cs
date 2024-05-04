using MushroomPocket.Models;

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

        // Ensure DB exists
        using (MushroomContext db = new MushroomContext())
        {
            db.Database.EnsureCreated();
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
                @"********************************",
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
                    AddCharacter();
                    break;

                case "2":
                    ListCharacters();
                    break;

                case "3":
                    CheckTransformation(mushroomMasters);
                    break;

                case "4":
                    TransformCharacters(mushroomMasters);
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


    // Option 1: Add character
    private static void AddCharacter()
    {
        // Name
        Console.Write("Enter Character's Name: ");
        string? charName = Console.ReadLine();

        if (!((string[])["Daisy", "Wario", "Waluigi"]).Contains(charName))
        {
            Console.WriteLine("\nInvalid character name. Please only enter ['Daisy', 'Wario', 'Waluigi'].");
            return;
        }

        // HP
        Console.Write("Enter Character's HP: ");
        float charHP;
        if (!float.TryParse(Console.ReadLine(), out charHP))
        {
            Console.WriteLine("\nInvalid HP. Please only enter a number.");
            return;
        }

        // Exp
        Console.Write("Enter Character's EXP: ");
        int charEXP;
        if (!int.TryParse(Console.ReadLine(), out charEXP))
        {
            Console.WriteLine("\nInvalid EXP. Please only enter an integer.");
            return;
        }

        // Add char
        string? errOut;
        Character? newChar = Character.From(charName!, charHP, charEXP, out errOut);
        if (errOut != null)
        {
            Console.WriteLine("\n" + errOut);
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            db.Characters.Add(newChar!);
            db.SaveChanges();
        }
        Console.WriteLine($"{charName} has been added.");
        return;
    }

    // Option 2: List Characters
    private static void ListCharacters()
    {
        // Sort descending
        List<Character> sorted;
        using (MushroomContext db = new MushroomContext())
        {
            sorted = db.Characters.OrderByDescending((Character c) => c.Hp).ToList();
        }

        foreach (Character c in sorted)
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
    }

    // Option 3: Check transformation
    private static void CheckTransformation(List<MushroomMaster> mushroomMasters)
    {
        // Check transformation
        List<MushroomMaster> canEvoList = Character.CanEvolve(mushroomMasters);
        foreach (MushroomMaster m in canEvoList)
        {
            Console.WriteLine($"{m.Name} -> {m.TransformTo}");
        }
    }

    // Option 4: Transform characters
    private static void TransformCharacters(List<MushroomMaster> mushroomMasters)
    {
        // Transform character
        List<MushroomMaster> evoList = Character.CanEvolve(mushroomMasters);

        using (MushroomContext db = new MushroomContext())
        {
            foreach (MushroomMaster m in evoList)
            {
                string? errOut2;
                Character? evoChar = Character.From(m.TransformTo, 100, 0, out errOut2, false);
                if (errOut2 != null)
                {
                    Console.WriteLine("\n" + errOut2);
                    break;
                }

                // Update DB
                db.Characters.RemoveRange(db.Characters.Where((Character c) => c.Name == m.Name).Take(m.NoToTransform).ToList());
                db.Characters.Add(evoChar!);
                Console.WriteLine($"{m.Name} has been transformed to {m.TransformTo}.");
            }

            db.SaveChanges();
        }
    }
}
