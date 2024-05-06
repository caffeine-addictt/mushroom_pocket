using MushroomPocket.Utils;
using MushroomPocket.Models;
using Microsoft.EntityFrameworkCore;

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
            new MushroomMaster("Waluigi", 1, "Luigi"),
        };

        // Validate MushroomMaster list
        List<string> violations = new List<string>();
        foreach (MushroomMaster m in mushroomMasters)
        {
            if (!Character.IsValidName(m.Name))
                violations.Add($"Attribute name {m.Name} in new MushroomMaster(\"{m.Name}\", \"{m.NoToTransform}\", \"{m.TransformTo}\") is invalid.");
            if (!Character.IsValidName(m.TransformTo))
                violations.Add($"Attribute transformTo {m.TransformTo} in new MushroomMaster(\"{m.Name}\", \"{m.NoToTransform}\", \"{m.TransformTo}\") is invalid.");
        }

        if (violations.Count > 0)
        {
            Console.WriteLine(String.Join("\n", new List<string>() {
                "Validating mushroomMasters failed. Please fix the following errors:",
            }.Concat(violations)));
            Environment.Exit(1);
        }

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
                @"(5). Delete character(s) from my pocket",
                @"(6). Manage my teams",
                @"Please only enter [1, 2, 3, 4, 5, 6] or Q to quit: "
            ]));

            switch ((Console.ReadLine() ?? "").ToLower())
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

                case "5":
                    DeleteCharacters();
                    break;

                case "6":
                    ManageTeams();
                    break;

                case "q":
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
        Similarity? topSuggestion;
        List<string> possibleNames = new List<string>() { "Daisy", "Wario", "Waluigi" };

        if (!StringUtils.SmartLookUp(Console.ReadLine() ?? "", possibleNames, out topSuggestion) || topSuggestion == null)
        {
            Console.WriteLine("\nInvalid character name. Please only enter ['Daisy', 'Wario', 'Waluigi'].");
            return;
        }

        if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
        {
            Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
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
        Character? newChar = Character.From(topSuggestion.QualifiedText, charHP, charEXP, out errOut);
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
        Console.WriteLine($"{topSuggestion.QualifiedText} has been added.");
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
                $"ID: {c.Id}",
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
        List<MushroomMaster> evolved = Character.Evolve(mushroomMasters);
        foreach (MushroomMaster m in evolved)
        {
            Console.WriteLine($"{m.Name} has been transformed to {m.TransformTo}.");
        }
    }

    // Option 5: Delete characters
    private static void DeleteCharacters()
    {
        // Ask for pattern
        Console.Write("Enter Character Name or ID [* for all]: ");
        string? delPattern = Console.ReadLine();

        if (String.IsNullOrWhiteSpace(delPattern))
        {
            Console.WriteLine("\nCharacter name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Character> delList =
                (delPattern! == "*")
                    ? db.Characters.ToList()
                    : db.Characters.Where((Character c) => c.Name.StartsWith(delPattern!) || c.Id.StartsWith(delPattern!)).ToList();

            // Check if empty
            if (delList.Count == 0)
            {
                Console.WriteLine("\nNo character(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {delList.Count} character(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;

            db.RemoveRange(delList);
            db.SaveChanges();
            Console.WriteLine($"{delList.Count} character(s) have been deleted.");
        }
    }

    // Option 6: Manage teams
    private static void ManageTeams()
    {
        // Ask for action.
        Console.Write(String.Join("\n", [
            @"",
            @"(1). Add a new team to my pocket",
            @"(2). Add Mushroom's character to a team",
            @"(3). List team(s) in my pocket",
            @"(4). List character(s) in a team",
            @"(5). Delete team(s) from my pocket",
            @"Please only enter [1, 2, 3, 4, 5]: "
        ]));
        string action = Console.ReadLine() ?? "";

        switch (action.ToLower())
        {
            case "1":
                AddTeam();
                break;

            case "2":
                AddCharacterToTeam();
                break;

            case "3":
                ListTeams();
                break;

            case "4":
                ListCharactersInTeam();
                break;

            case "5":
                DeleteTeams();
                break;

            default:
                Console.WriteLine("\nInvalid action. Please only enter [1, 2, 3, 4, 5] or Q to quit.");
                break;
        }
    }

    // Option 6-1: Add new team
    private static void AddTeam()
    {
        // Name
        Console.Write("Enter new team name: ");
        string teamName = (Console.ReadLine() ?? "").Trim();
        if (String.IsNullOrWhiteSpace(teamName))
        {
            Console.WriteLine("\nTeam name cannot be empty.");
            return;
        }

        // Desc
        Console.Write("Enter a team description: ");
        string teamDesc = (Console.ReadLine() ?? "").Trim();

        using (MushroomContext db = new MushroomContext())
        {
            if (db.Teams.Where((Team t) => t.Name == teamName).Count() > 0)
            {
                Console.WriteLine("\nTeam name already exists.");
                return;
            }
            db.Teams.Add(new Team(teamName, teamDesc));
            db.SaveChanges();
        }

        Console.WriteLine($"{teamName} has been added.");
    }

    // Option 6-2: Add character to team
    private static void AddCharacterToTeam()
    {
        // Ask for pattern
        Console.Write("Enter Character Name or ID [* for all]: ");
        string namePattern = Console.ReadLine() ?? "";

        if (String.IsNullOrWhiteSpace(namePattern))
        {
            Console.WriteLine("\nCharacter name or ID cannot be empty.");
            return;
        }

        // Ask for team pattern
        Console.Write("Enter Team Name or ID [* for all]: ");
        string teamPattern = Console.ReadLine() ?? "";
        if (String.IsNullOrWhiteSpace(teamPattern))
        {
            Console.WriteLine("\nTeam name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Character> charList =
                namePattern == "*"
                ? db.Characters.ToList()
                : db.Characters.Where((Character c) => c.Name.StartsWith(namePattern) || c.Id.StartsWith(namePattern)).ToList();

            HashSet<Team> teamList =
                teamPattern == "*"
                ? db.Teams.Include(t => t.Characters).ToHashSet()
                : db.Teams.Include(t => t.Characters).Where((Team t) => t.Name.StartsWith(teamPattern) || t.Id.StartsWith(teamPattern)).ToHashSet();

            // Check if empty
            if (charList.Count == 0)
            {
                Console.WriteLine("\nNo character(s) found. Nothing to add to team.");
                return;
            }
            if (teamList.Count == 0)
            {
                Console.WriteLine("\nNo team(s) found. Nothing to add to team.");
                return;
            }

            // Confirmation
            Console.Write($"Are you sure you want to add {charList.Count} character(s) to {teamList.Count} team(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;

            // Add
            bool hasChange = false;
            foreach (Team t in teamList)
            {
                if (t.Characters.Intersect(t.Characters).Count() > 0)
                {
                    if (teamPattern == "*") continue;
                    Console.WriteLine($"\nTeam {t.Name} already has the character(s) {String.Join(", ", t.Characters)}.");
                    return;
                }

                hasChange = true;
                t.AddCharacterRange(charList);
            }

            if (!hasChange)
            {
                Console.WriteLine($"\nCharacter(s) {String.Join(", ", charList)} already exist in team(s).");
                return;
            }

            db.SaveChanges();
            Console.WriteLine($"{charList.Count} character(s) have been added to {teamList.Count} team(s).");
        }
    }

    // Option 6-3: List team(s)
    private static void ListTeams()
    {
        // Sort descending
        List<Team> sorted;
        using (MushroomContext db = new MushroomContext())
        {
            sorted = db.Teams.Include(t => t.Characters).ToList();
            Console.WriteLine(sorted[0].Characters);
        }
        sorted.Sort((Team t1, Team t2) => t2.Characters.Count.CompareTo(t1.Characters.Count));

        foreach (Team t in sorted)
        {
            Console.WriteLine(String.Join("\n",
                @"-----------------------",
                $"ID: {t.Id}",
                $"Name: {t.Name}",
                $"Description: {t.Description}",
                $"Character(s): {t.Characters.Count}",
                @"-----------------------"
            ));
        }
    }

    // Option 6-4: List character(s) in team
    private static void ListCharactersInTeam()
    {
        // Ask for pattern
        Console.Write("Enter Team Name or ID: ");
        string teamPattern = Console.ReadLine() ?? "";

        using (MushroomContext db = new MushroomContext())
        {
            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(teamPattern, db.Teams.Select((Team t) => t.Name).ToList(), out topSuggestion!))
            {
                Console.WriteLine("\nTeam name or ID not found.");
                return;
            }

            // Confirmation if not the same
            if (StringUtils.Clean(teamPattern, true) != StringUtils.Clean(topSuggestion.QualifiedText, true))
            {
                Console.Write($"Are you sure you want to list character(s) in team {topSuggestion.QualifiedText}? [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            // Stdout characters
            List<Character> charList = db.Teams.Include(t => t.Characters).Where((Team t) => t.Name == topSuggestion.QualifiedText).First()!.Characters.ToList();
            charList.Sort((Character c1, Character c2) => c2.Hp.CompareTo(c1.Hp));

            foreach (Character c in charList)
            {
                Console.WriteLine(String.Join("\n", [
                    @"-----------------------",
                    $"ID: {c.Id}",
                    $"Name: {c.Name}",
                    $"HP: {c.Hp}",
                    $"EXP: {c.Exp}",
                    $"Skill: {c.Skill}",
                    @"-----------------------"
                ]));
            }
        }
    }

    // Option 6-5: Delete team(s)
    private static void DeleteTeams()
    {
        // Ask for pattern
        Console.Write("Enter Team Name or ID [* for all]: ");
        string? delPattern = Console.ReadLine();

        if (String.IsNullOrWhiteSpace(delPattern))
        {
            Console.WriteLine("\nTeam name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Team> delList =
                (delPattern! == "*")
                    ? db.Teams.ToList()
                    : db.Teams.Where((Team t) => t.Name.StartsWith(delPattern!) || t.Id.StartsWith(delPattern!)).ToList();

            // Check if empty
            if (delList.Count == 0)
            {
                Console.WriteLine("\nNo team(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {delList.Count} team(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;

            db.RemoveRange(delList);
            db.SaveChanges();
            Console.WriteLine($"{delList.Count} team(s) have been deleted.");
        }
    }
}
