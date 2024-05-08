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


public static class ManageTeams
{
    public static readonly string InterfaceText = String.Join(
        "\n",
        [
            @"",
            @"(1). Add a new team to my pocket",
            @"(2). Add Mushroom's character to a team",
            @"(3). List team(s) in my pocket",
            @"(4). List character(s) in a team",
            @"(5). Delete team(s) from my pocket",
            @"Please only enter [1, 2, 3, 4, 5] or b to go back: "
        ]
    );


    // Main entry point
    public static void MainEntry()
    {
        Console.Write(InterfaceText);
        switch ((Console.ReadLine() ?? "").ToLower())
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

            case "b":
                break;

            default:
                Console.WriteLine(
                    "\nInvalid action. Please only enter [1, 2, 3, 4, 5] or Q to quit."
                );
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
            Profile profile = db.GetProfile(true);
            if (profile.Teams.Where((Team t) => t.Name == teamName).Count() > 0)
            {
                Console.WriteLine("\nTeam name already exists.");
                return;
            }
            profile.Teams.Add(new Team(teamName, teamDesc));
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
            Profile profile = db.GetProfile(true, true);
            List<Character> charList =
                namePattern == "*"
                    ? profile.Characters.ToList()
                    : profile.Characters.Where(
                            (Character c) =>
                                c.Name.StartsWith(namePattern) || c.Id.StartsWith(namePattern)
                        )
                        .ToList();

            HashSet<Team> teamList =
                teamPattern == "*"
                    ? profile.Teams.ToHashSet()
                    : profile.Teams
                        .Where(
                            (Team t) =>
                                t.Name.StartsWith(teamPattern) || t.Id.StartsWith(teamPattern)
                        )
                        .ToHashSet();

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
            Console.Write(
                $"Are you sure you want to add {charList.Count} character(s) to {teamList.Count} team(s)? [Y/N]: "
            );
            if ((Console.ReadLine() ?? "").ToLower() != "y")
                return;

            // Add
            bool hasChange = false;
            foreach (Team t in teamList)
            {
                if (t.Characters.Intersect(t.Characters).Count() > 0)
                {
                    if (teamPattern == "*")
                        continue;
                    Console.WriteLine(
                        $"\nTeam {t.Name} already has the character(s) {String.Join(", ", t.Characters)}."
                    );
                    return;
                }

                hasChange = true;
                t.AddCharacterRange(charList);
            }

            if (!hasChange)
            {
                Console.WriteLine(
                    $"\nCharacter(s) {String.Join(", ", charList)} already exist in team(s)."
                );
                return;
            }

            db.SaveChanges();
            Console.WriteLine(
                $"{charList.Count} character(s) have been added to {teamList.Count} team(s)."
            );
        }
    }

    // Option 6-3: List team(s)
    private static void ListTeams()
    {
        // Sort descending
        List<Team> sorted;
        using (MushroomContext db = new MushroomContext())
        {
            sorted = db.GetProfile(true, true).Teams.ToList();
        }
        sorted.Sort((Team t1, Team t2) => t2.Characters.Count.CompareTo(t1.Characters.Count));

        if (sorted.Count == 0)
        {
            Console.WriteLine("\nNo team(s) to list!");
            return;
        }

        foreach (Team t in sorted)
        {
            Console.WriteLine(
                String.Join(
                    "\n",
                    @"-----------------------",
                    $"ID: {t.Id}",
                    $"Name: {t.Name}",
                    $"Description: {t.Description}",
                    $"Character(s): {t.Characters.Count}",
                    @"-----------------------"
                )
            );
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
            if (
                !StringUtils.SmartLookUp(
                    teamPattern,
                    db.Teams.Select((Team t) => t.Name).ToList(),
                    out topSuggestion!
                )
            )
            {
                Console.WriteLine("\nTeam name or ID not found.");
                return;
            }

            // Confirmation if not the same
            if (
                StringUtils.Clean(teamPattern, true)
                != StringUtils.Clean(topSuggestion.QualifiedText, true)
            )
            {
                Console.Write($"\nDid you mean {topSuggestion.QualifiedText}? [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y")
                    return;
            }

            // Stdout characters
            List<Character> charList = db
                .GetProfile(true)
                .Teams
                .Where((Team t) => t.Name == topSuggestion.QualifiedText)
                .First()!
                .Characters.ToList();
            charList.Sort((Character c1, Character c2) => c2.Hp.CompareTo(c1.Hp));

            if (charList.Count == 0)
            {
                Console.WriteLine("\nNo character(s) to list!");
                return;
            }

            foreach (Character c in charList)
            {
                Console.WriteLine(
                    String.Join(
                        "\n",
                        [
                            @"-----------------------",
                            $"ID: {c.Id}",
                            $"Name: {c.Name}",
                            $"HP: {c.Hp}",
                            $"EXP: {c.Exp}",
                            $"Skill: {c.Skill}",
                            @"-----------------------"
                        ]
                    )
                );
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
            Profile profile = db.GetProfile(true);
            List<Team> delList =
                (delPattern! == "*")
                    ? profile.Teams.ToList()
                    : profile
                        .Teams.Where(
                            (Team t) =>
                                t.Name.StartsWith(delPattern!) || t.Id.StartsWith(delPattern!)
                        )
                        .ToList();

            // Check if empty
            if (delList.Count == 0)
            {
                Console.WriteLine("\nNo team(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {delList.Count} team(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y")
                return;

            profile.Teams.ExceptWith(delList);
            db.SaveChanges();
            Console.WriteLine($"{delList.Count} team(s) have been deleted.");
        }
    }
}
