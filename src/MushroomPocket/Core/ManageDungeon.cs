/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

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
                break;

            case "3":
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
