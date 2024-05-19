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


public static class ManageDungeon
{
    public static readonly string InterfaceText = String.Join(
        "\n",
        @"",
        @"(1). Enter a dungeon",
        @"(2). View dungeon(s)",
        @"(3). View battle log",
        @"Please only enter [1, 2, 3] or b to go back: "
    );


    // Handle dungeon spawn
    public static DateTime GetNextDungeonSpawn(DateTime currentTime)
        => currentTime.AddMinutes(1);
    public static void HandleDungeonSpawn()
    {
        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(IncludeFlags.Dungeons);

            if (profile.NextDungeonSpawn > DateTime.UtcNow)
                return;

            profile.NextDungeonSpawn = GetNextDungeonSpawn(DateTime.UtcNow);

            // New Dungeon
            Dungeon dungeon = new Dungeon();
            Console.WriteLine($"\n{dungeon.GetDifficulty()} Rank Dungeon \"{dungeon.Name}\" has revealed itself!");

            profile.Dungeons.Add(dungeon);
            db.SaveChanges();
        }
    }


    // Echo
    public static void EchoDungeon(Dungeon d)
        => Console.WriteLine(String.Join(
            "\n",
            @"-----------------------",
            $"ID: {d.Id}",
            $"Rank: {d.GetDifficulty()}",
            $"Status: {d.Status}",
            $"Name: {d.Name}",
            $"Description: {d.Description}",
            (d.Status == "Unopened")
                ? $"Bribe needed: ${d.EntryCost}\n-----------------------"
                : @"-----------------------"
        ));


    // Main entry point
    public static void MainEntry()
    {
        Console.Write(InterfaceText);
        switch ((Console.ReadLine() ?? "").Trim())
        {
            case "1":
                EnterDungeon();
                break;

            case "2":
                ListDungeon();
                break;

            case "3":
                ListBattleLog();
                break;

            case "b":
                return;

            default:
                Console.WriteLine("\nInvalid action. Please enter only [1, 2, 3] or b to go back.");
                break;
        }
    }

    // Option 9-1: Enter a dungeon
    private static void EnterDungeon()
    {
        Team team;
        Dungeon dungeon;

        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(IncludeFlags.Dungeons);
            List<Dungeon> dungeonList = profile.Dungeons.Where(d => d.Status != "Cleared").ToList();
            if (dungeonList.Count == 0)
            {
                Console.WriteLine("\nNo dungeons have been discovered! Keep playing and dungeons might show itself!");
                return;
            }

            Console.Write("Enter Dungeon ID or L to list all dungeons: ");
            string dungeonId = (Console.ReadLine() ?? "").Trim();

            if (dungeonId.ToLower() == "l")
            {
                foreach (Dungeon d in dungeonList)
                    EchoDungeon(d);

                Console.Write("Enter Dungeon ID: ");
                dungeonId = (Console.ReadLine() ?? "").Trim();
            }

            Similarity topDungeonSuggestion;
            if (!StringUtils.SmartLookUp(dungeonId, dungeonList.Select(d => d.Id), out topDungeonSuggestion!))
            {
                Console.WriteLine("\nDungeon ID not found!");
                return;
            }

            if (topDungeonSuggestion.QualifiedText.ToLower() != topDungeonSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topDungeonSuggestion.QualifiedText}'? ({topDungeonSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            // Get team to use
            List<Team> teamList = db.GetTeams(IncludeFlags.TeamCharacters).ToList();
            if (teamList.Count == 0)
            {
                Console.WriteLine("\nYou do not have any team. Create one first!");
                return;
            }

            Console.Write("Enter team ID or L to list all team: ");
            string teamId = (Console.ReadLine() ?? "").Trim();

            if (teamId.ToLower() == "l")
            {
                foreach (Team t in teamList)
                    ManageTeams.EchoTeam(t);

                Console.Write("Enter team ID: ");
                teamId = (Console.ReadLine() ?? "").Trim();
            }

            Similarity topTeamSuggestion;
            if (!StringUtils.SmartLookUp(teamId, teamList.Select(t => t.Id), out topTeamSuggestion!))
            {
                Console.WriteLine("\nTeam not found!");
                return;
            }

            if (topTeamSuggestion.QualifiedText.ToLower() != topTeamSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topTeamSuggestion.QualifiedText}'? ({topTeamSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            // Affected Dungeon and Team
            team = teamList.First(t => t.Id == topTeamSuggestion.QualifiedText);
            dungeon = dungeonList.First(d => d.Id == topDungeonSuggestion.QualifiedText);

            // Return if no characters in team
            if (team.Characters.Count == 0)
            {
                Console.WriteLine("\nNo character in this team. Add some first!");
                return;
            }

            // Return if a character is dead
            foreach (Character c in team.Characters)
            {
                if (c.Hp <= 0)
                {
                    Console.WriteLine($"\n{c.Name} [{c.Id}] is dead, causing the team morale to be low! You won't be able to clear this dungeon!\nEither heal {c.Name} or use a difference team!");
                    return;
                }
            }

            // Return if too expensive
            if (profile.Wallet < dungeon.EntryCost)
            {
                Console.WriteLine(dungeon.UnlockReject(profile.Wallet));
                return;
            }

            Console.Write(dungeon.UnlockAsk);
            if ((Console.ReadLine() ?? "").Trim().ToLower() != "y") return;
            profile.Wallet -= dungeon.EntryCost;
            Console.WriteLine(dungeon.UnlockSuccess(profile.Wallet));
            db.SaveChanges();
        }

        DungeonGameLogic.GameLogic.Start(team, dungeon);
    }

    // Option 9-2: View dungeon(s)
    private static void ListDungeon()
    {
        using (MushroomContext db = new MushroomContext())
        {
            IEnumerable<Dungeon> dungeonList = db.GetDungeons();

            // Early length check
            if (dungeonList.Count() == 0)
            {
                Console.WriteLine("\nNo dungeons to list!");
                return;
            }

            // Handle difficulty filter
            Console.Write("Do you want to filter by difficulty? [Y/N]: ");
            if ((Console.ReadLine() ?? "").Trim().ToLower() == "y")
            {
                Console.Write("Enter difficulty [S, A, B, C, D]: ");
                string difficulty = (Console.ReadLine() ?? "").Trim().ToUpper();

                if (new List<string>(["S", "A", "B", "C", "D"]).Contains(difficulty))
                {
                    dungeonList = dungeonList.Where(d => d.Difficulty == Dungeon.GetDifficultyNum(difficulty));
                }
                else
                {
                    Console.WriteLine("\nInvalid difficulty. Please enter only [S, A, B, C, D].");
                    return;
                }
            }

            // Handle status filter
            Console.Write("Do you want to filter by status? [Y/N]: ");
            if ((Console.ReadLine() ?? "").Trim().ToLower() == "y")
            {
                Console.Write("Enter status [Uncleared, Cleared]: ");
                Similarity topSuggestion;
                if (!StringUtils.SmartLookUp((Console.ReadLine() ?? "").Trim(), new List<string>(["Uncleared", "Cleared"]), out topSuggestion!))
                {
                    Console.WriteLine("\nInvalid status. Please enter only [Uncleared, Cleared].");
                    return;
                }

                if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
                {
                    Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                    if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                }

                dungeonList = dungeonList.Where(d => d.Status == topSuggestion.QualifiedText);
            }

            if (dungeonList.Count() == 0)
            {
                Console.WriteLine("\nNo dungeons to list!");
                return;
            }

            foreach (Dungeon d in dungeonList)
                EchoDungeon(d);
        }
    }

    // Option 9-3: View battle log
    private static void ListBattleLog()
    {
        using (MushroomContext db = new MushroomContext())
        {
            List<BattleLog> battleLogs = db.GetBattleLogs().ToList();

            if (battleLogs.Count == 0)
            {
                Console.WriteLine("\nNo battle logs to list!");
                return;
            }

            foreach (BattleLog b in battleLogs)
                Console.WriteLine(String.Join(
                    "\n",
                    @"-----------------------",
                    b.ClearedDungeon ? "CLEAR" : "FAIL",
                    $"Difficulty: {b.DungeonDifficulty}",
                    $"Characters used: {b.CharactersUsed}",
                    $"Characters dead: {b.CharactersDead}",
                    $"Total Damage Dealt: {b.TotalDamageDealt}",
                    $"Total Damage Taken: {b.TotalDamageTaken}",
                    $"Date: {b.Date.ToShortDateString()}",
                    @"-----------------------"
                ));
        }
    }
}
