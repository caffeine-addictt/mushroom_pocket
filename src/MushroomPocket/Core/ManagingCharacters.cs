/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;
using MushroomPocket.Utils;

namespace MushroomPocket.Core;


public static class ManageCharacters
{
    // Abstract echo
    public static void EchoCharacter(Character c)
        => Console.WriteLine(String.Join(
            "\n",
            @"-----------------------",
            $"ID: {c.Id}",
            $"Name: {c.Name}",
            $"HP: {c.Hp}",
            $"EXP: {c.Exp}",
            $"Skill: {c.Skill}",
            @"-----------------------"
        ));




    // Option 1: Add character
    public static void AddCharacter()
    {
        // Name
        Console.Write("Enter Character's Name: ");
        Similarity? topSuggestion;
        List<string> possibleNames = new List<string>() { "Daisy", "Wario", "Waluigi" };

        if (
            !StringUtils.SmartLookUp(Console.ReadLine() ?? "", possibleNames, out topSuggestion)
            || topSuggestion == null
        )
        {
            Console.WriteLine(
                "\nInvalid character name. Please only enter ['Daisy', 'Wario', 'Waluigi']."
            );
            return;
        }

        if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
        {
            Console.Write(
                $"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: "
            );
            if ((Console.ReadLine() ?? "").ToLower() != "y")
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
        Character? newChar = Character.From(
            topSuggestion.QualifiedText,
            charHP,
            charEXP,
            out errOut
        );
        if (errOut != null)
        {
            Console.WriteLine("\n" + errOut);
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            db.GetProfile(false, true).Characters.Add(newChar!);
            db.SaveChanges();
        }
        Console.WriteLine($"{topSuggestion.QualifiedText} has been added.");
        Economy.AwardMoney(10);
        Console.WriteLine($"[+$10]");
        return;
    }

    // Option 2: List Characters
    public static void ListCharacters()
    {
        // Sort descending
        List<Character> sorted;
        using (MushroomContext db = new MushroomContext())
        {
            sorted = db.GetProfile(false, true).Characters.OrderByDescending((Character c) => c.Hp).ToList();
        }

        if (sorted.Count == 0)
        {
            Console.WriteLine("\nNo character(s) to list!");
            return;
        }

        // Stdout metrics
        Dictionary<string, int> collision = new Dictionary<string, int>();

        foreach (Character c in sorted)
        {
            collision[c.Name] = collision.ContainsKey(c.Name) ? collision[c.Name] + 1 : 1;
            EchoCharacter(c);
        }

        Console.WriteLine(String.Join(
            "\n",
            collision.Select((kv) => $"[x{kv.Value}] {kv.Key}")
        ));
    }

    // Option 3: Check transformation
    public static void CheckTransformation(List<MushroomMaster> mushroomMasters)
    {
        // Check transformation
        List<MushroomMaster> canEvoList = Character.CanEvolve(mushroomMasters);
        foreach (MushroomMaster m in canEvoList)
        {
            Console.WriteLine($"{m.Name} -> {m.TransformTo}");
        }
    }

    // Option 4: Transform characters
    public static void TransformCharacters(List<MushroomMaster> mushroomMasters)
    {
        // Transform character
        List<MushroomMaster> evolved = Character.Evolve(mushroomMasters);
        foreach (MushroomMaster m in evolved)
        {
            Console.WriteLine($"{m.Name} has been transformed to {m.TransformTo}.");
        }
    }

    // Option 5: Delete characters
    public static void DeleteCharacters()
    {
        Console.Write(String.Join(
            "\n",
            @"",
            @"(1). Delete character from ID",
            @"(2). Delete character from Name",
            @"(3). Delete character from pattern",
            @"(4). Delete all characters",
            @"Please only enter [1, 2, 3, 4] or b to go back: "
        ));

        switch ((Console.ReadLine() ?? "").ToLower())
        {
            case "1":
                DeleteCharacterFromID();
                break;

            case "2":
                DeleteCharacterFromName();
                break;

            case "3":
                DeleteCharacterFromPattern();
                break;

            case "4":
                DeleteAllCharacters();
                break;

            case "b":
                return;

            default:
                Console.WriteLine("\nInvalid input. Please only enter [1, 2, 3, 4] or b to go back.");
                break;
        }
    }

    // Option 5-1: Delete from ID
    private static void DeleteCharacterFromID()
    {
        // Ask for ID
        Console.Write("Enter Character ID: ");
        string id = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("\nCharacter ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(false, true);
            List<Character> characters = profile.Characters.ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(id, characters.Select(c => c.Id.ToString()), out topSuggestion!))
            {
                Console.WriteLine("\nCharacter not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Character delChar = characters.Where(c => c.Id == topSuggestion.QualifiedText).First();

            db.Characters.Remove(delChar);
            db.SaveChanges();

            Console.WriteLine($"Deleted character: {delChar.Name}");
        }
    }

    // Option 5-2: Delete from Name
    private static void DeleteCharacterFromName()
    {
        // Ask for name
        Console.Write("Enter Character Name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nCharacter Name cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(false, true);
            List<Character> characters = profile.Characters.ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(name, characters.Select(c => c.Name), out topSuggestion!))
            {
                Console.WriteLine("\nCharacter not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Character delChar = characters.Where(c => c.Name == topSuggestion.QualifiedText).First();
            db.Characters.Remove(delChar);
            db.SaveChanges();

            Console.WriteLine($"Deleted character: {delChar.Name}");
        }
    }

    // Option 5-3: Delete from pattern
    private static void DeleteCharacterFromPattern()
    {
        // Ask for pattern
        Console.Write("Enter Character Name or ID: ");
        string pattern = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(pattern))
        {
            Console.WriteLine("\nCharacter name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Character> characters = db.GetCharacters(true)
                .Where(c =>
                    c.Id.ToLower().StartsWith(pattern)
                    || c.Name.ToLower().StartsWith(pattern)
                )
                .ToList();

            if (characters.Count == 0)
            {
                Console.WriteLine("\nNo character(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {characters.Count} character(s)? [Y/N] or L to list the affected character(s): ");
            switch ((Console.ReadLine() ?? "").ToLower())
            {
                case "l":
                    foreach (Character c in characters)
                        EchoCharacter(c);

                    // Final confirmation
                    Console.Write("Are you sure you want to delete these characters? [Y/N]: ");
                    if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                    break;
                case "y":
                    break;

                default:
                    return;
            }

            // Delete
            db.Characters.RemoveRange(characters);
            db.SaveChanges();
            Console.WriteLine($"Deleted {characters.Count} character(s).");
        }
    }

    // Option 5-4: Delete all
    private static void DeleteAllCharacters()
    {
        using (MushroomContext db = new MushroomContext())
        {
            List<Character> charList = db.GetCharacters().ToList();

            Console.Write($"Are you sure you want to delete {charList.Count()} character(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            db.Characters.RemoveRange(charList);
            db.SaveChanges();

            Console.WriteLine("All characters deleted.");
        }
    }
}
