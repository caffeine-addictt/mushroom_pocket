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

        foreach (Character c in sorted)
        {
            Console.WriteLine(
                String.Join(
                    "\n",
                    @"-----------------------",
                    $"ID: {c.Id}",
                    $"Name: {c.Name}",
                    $"HP: {c.Hp}",
                    $"EXP: {c.Exp}",
                    $"Skill: {c.Skill}",
                    @"-----------------------"
                )
            );
        }
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
            Profile profile = db.GetProfile(false, true);
            List<Character> delList =
                (delPattern! == "*")
                    ? profile.Characters.ToList()
                    : profile
                        .Characters.Where(
                            (Character c) =>
                                c.Name.StartsWith(delPattern!) || c.Id.StartsWith(delPattern!)
                        )
                        .ToList();

            // Check if empty
            if (delList.Count == 0)
            {
                Console.WriteLine("\nNo character(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {delList.Count} character(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y")
                return;

            profile.Characters.ExceptWith(delList);
            db.SaveChanges();
            Console.WriteLine($"{delList.Count} character(s) have been deleted.");
        }
    }
}
