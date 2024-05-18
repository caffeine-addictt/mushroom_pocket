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

    // Option 9-2: View dungeon(s)
    private static void ListDungeon()
    {
        using (MushroomContext db = new MushroomContext())
        {
            IEnumerable<Dungeon> dungeonList = db.GetDungeons();

            // Early length check
            if (dungeonList.Count() == 0)
            {
                Console.WriteLine("\nNo dungeon to list!");
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
