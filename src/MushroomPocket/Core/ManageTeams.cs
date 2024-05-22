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


    // Echo
    public static void EchoTeam(Team t)
        => Console.WriteLine(String.Join(
            "\n",
            @"-----------------------",
            $"ID: {t.Id}",
            $"Name: {t.Name}",
            $"Description: {t.Description}",
            $"Character(s): {t.Characters.Count}",
            @"-----------------------"
        ));


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
            Profile profile = db.GetProfile(IncludeFlags.Teams);
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
        using (MushroomContext db = new MushroomContext())
        {
            List<Character> charList = db.GetCharacters().ToList();
            List<Team> teamList = db.GetTeams(IncludeFlags.TeamCharacters).ToList();

            // check for char and team
            if (charList.Count == 0)
            {
                Console.WriteLine("\nNo character(s) to add.");
                return;
            }
            if (teamList.Count == 0)
            {
                Console.WriteLine("\nNo team(s) to add.");
                return;
            }

            // Ask for team pattern
            Console.Write("Enter Team Name or ID or L to list all teams: ");
            string teamNameID = Console.ReadLine() ?? "";
            if (teamNameID.ToLower() == "l")
            {
                foreach (Team t in teamList)
                    ManageTeams.EchoTeam(t);

                // Confirmation
                Console.Write("Enter Team Name or ID: ");
                teamNameID = (Console.ReadLine() ?? "").Trim();
            }

            Similarity topTeamID;
            if (!StringUtils.SmartLookUp(teamNameID, teamList.Select(t => t.Name).Union(teamList.Select(t => t.Id)), out topTeamID!))
            {
                Console.WriteLine("\nTeam Name or ID does not exist.");
                return;
            }

            if (topTeamID.QualifiedText.ToLower() != topTeamID.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topTeamID.QualifiedText}'? ({topTeamID.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Team affectedTeam = teamList.First(t => t.Id == topTeamID.QualifiedText);
            charList = charList.Where(c => affectedTeam.Characters.All(tc => tc.Id != c.Id)).ToList();
            if (charList.Count == 0)
            {
                Console.WriteLine("\nNo character(s) to add.");
                return;
            }


            // Ask for character pattern
            Console.Write("Enter Character ID or L to list all characters: ");
            string charID = (Console.ReadLine() ?? "").Trim();
            if (charID.ToLower() == "l")
            {
                foreach (Character c in charList)
                    ManageCharacters.EchoCharacter(c);

                // Confirmation
                Console.Write("Enter Character ID: ");
                charID = (Console.ReadLine() ?? "").Trim();
            }

            Similarity charTopID;
            if (!StringUtils.SmartLookUp(charID, charList.Select(c => c.Id), out charTopID!))
            {
                Console.WriteLine("\nCharacter ID does not exist.");
                return;
            }

            if (charTopID.QualifiedText.ToLower() != charTopID.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{charTopID.QualifiedText}'? ({charTopID.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }


            Character affectedCharacter = charList.First(c => c.Id == charTopID.QualifiedText);

            // Check team limits
            if (affectedTeam.Characters.Count == 5)
            {
                Console.WriteLine("\nTeam can have only a maximum of 5 characters");
                return;
            }

            // Check colision
            if (affectedTeam.Characters.Any(c => c.Id == affectedCharacter.Id))
            {
                Console.WriteLine("\nCharacter is already in the team!");
                return;
            }

            // Confirmation
            Console.Write($"Are you sure you want to add Character {affectedCharacter.Id} in Team {affectedTeam.Id}? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y")
                return;

            // Add
            affectedTeam.Characters.Add(affectedCharacter);
            db.SaveChanges();

            Console.WriteLine($"{affectedCharacter.Id} added to team {affectedTeam.Id}!");
        }
    }

    // Option 6-3: List team(s)
    private static void ListTeams()
    {
        // Sort descending
        List<Team> sorted;
        using (MushroomContext db = new MushroomContext())
        {
            sorted = db.GetTeams(IncludeFlags.Characters).ToList();
        }
        sorted.Sort((Team t1, Team t2) => t2.Characters.Count.CompareTo(t1.Characters.Count));

        if (sorted.Count == 0)
        {
            Console.WriteLine("\nNo team(s) to list!");
            return;
        }

        foreach (Team t in sorted)
            EchoTeam(t);
    }

    // Option 6-4: List character(s) in team
    private static void ListCharactersInTeam()
    {
        // Ask for pattern
        Console.Write("Enter Team Name or ID: ");
        string teamPattern = Console.ReadLine() ?? "";

        using (MushroomContext db = new MushroomContext())
        {
            List<Team> teamList = db.GetTeams(IncludeFlags.Characters).ToList();

            Similarity topSuggestion;
            if (
                !StringUtils.SmartLookUp(
                    teamPattern,
                    teamList.Select((Team t) => t.Name).Union(teamList.Select((Team t) => t.Id)),
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
            List<Character> charList = teamList
                .Where((Team t) => t.Name == topSuggestion.QualifiedText || t.Id == topSuggestion.QualifiedText)
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
        Console.Write(String.Join(
            "\n",
            @"",
            @"(1). Delete team from ID",
            @"(2). Delete team from Name",
            @"(3). Delete team from pattern",
            @"(4). Delete all teams",
            @"Please only enter [1, 2, 3, 4] or b to go back: "
        ));

        switch ((Console.ReadLine() ?? "").ToLower())
        {
            case "1":
                DeleteTeamFromID();
                break;

            case "2":
                DeleteTeamFromName();
                break;

            case "3":
                DeleteTeamFromPattern();
                break;

            case "4":
                DeleteAllTeams();
                break;

            case "b":
                return;

            default:
                Console.WriteLine("\nInvalid input. Please only enter [1, 2, 3, 4] or b to go back.");
                break;
        }
    }

    // Option 5-1: Delete from ID
    private static void DeleteTeamFromID()
    {
        // Ask for ID
        Console.Write("Enter Team ID: ");
        string id = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("\nTeam ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Team> teams = db.GetTeams().ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(id, teams.Select(t => t.Id.ToString()), out topSuggestion!))
            {
                Console.WriteLine("\nTeam not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Team delTeam = teams.Where(c => c.Id == topSuggestion.QualifiedText).First();

            db.Teams.Remove(delTeam);
            db.SaveChanges();

            Console.WriteLine($"Deleted team: {delTeam.Name}");
        }
    }

    // Option 5-2: Delete from Name
    private static void DeleteTeamFromName()
    {
        // Ask for name
        Console.Write("Enter Team Name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nTeam Name cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Team> teams = db.GetTeams().ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(name, teams.Select(t => t.Name), out topSuggestion!))
            {
                Console.WriteLine("\nTeam not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Team delTeam = teams.Where(t => t.Name == topSuggestion.QualifiedText).First();
            db.Teams.Remove(delTeam);
            db.SaveChanges();

            Console.WriteLine($"Deleted team: {delTeam.Name}");
        }
    }

    // Option 5-3: Delete from pattern
    private static void DeleteTeamFromPattern()
    {
        // Ask for pattern
        Console.Write("Enter Team Name or ID: ");
        string pattern = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(pattern))
        {
            Console.WriteLine("\nTeam name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Team> teams = db.GetTeams(IncludeFlags.Characters)
                .Where(t =>
                    t.Id.ToLower().StartsWith(pattern)
                    || t.Name.ToLower().StartsWith(pattern)
                )
                .ToList();

            if (teams.Count == 0)
            {
                Console.WriteLine("\nNo team(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {teams.Count} team(s)? [Y/N] or L to list the affected team(s): ");
            switch ((Console.ReadLine() ?? "").ToLower())
            {
                case "l":
                    foreach (Team t in teams)
                        EchoTeam(t);

                    // Final confirmation
                    Console.Write("Are you sure you want to delete these teams? [Y/N]: ");
                    if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                    break;
                case "y":
                    break;

                default:
                    return;
            }

            // Delete
            db.Teams.RemoveRange(teams);
            db.SaveChanges();
            Console.WriteLine($"Deleted {teams.Count} team(s).");
        }
    }

    // Option 5-4: Delete all
    private static void DeleteAllTeams()
    {
        using (MushroomContext db = new MushroomContext())
        {
            List<Team> teamList = db.GetTeams().ToList();

            Console.Write($"Are you sure you want to delete {teamList.Count()} team(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            db.Teams.RemoveRange(teamList);
            db.SaveChanges();

            Console.WriteLine("All teams deleted.");
        }
    }
}
