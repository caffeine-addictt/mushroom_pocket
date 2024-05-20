/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

/* using System.Threading; */
using MushroomPocket.Models;
/* using MushroomPocket.Utils; */

namespace MushroomPocket.Core.DungeonGameLogic;


public static class GameLogic
{
    public static void Start(MushroomContext db, Team team, Dungeon dungeon)
    {
        // Threaded so I can setup some stuff while it shows loading screen in the meantime
        LoadingHandler initialLoadHandler = Loading.Start();
        DungeonMaster dm = new DungeonMaster(dungeon);
        ExplorationParty party = new ExplorationParty(team);

        // Dispose loading screen
        Thread.Sleep(3000);
        initialLoadHandler.Dispose();

        // Helper functions
        bool ContinueGame() => party.IsAllAlive() && dm.Hp != 0;

        // Game loop
        while (ContinueGame())
        {
            // DM moves first
            Frame.DrawFrame(team, dm);
            Console.WriteLine("DM's turn");

            PartyMember target = party.PickRandomMember();
            float damage = dm.RollDamage();
            target.TakeDamage(damage);

            // Redraw
            Frame.DrawFrame(team, dm);
            Console.WriteLine($"DM hits {target.Character.Name} [{target.Character.Id}] with {damage} damage. [{target.Character.Hp}] HP remaining.");

            Console.WriteLine(damage);
            Thread.Sleep(3000);


            // Characters' move
            foreach (PartyMember member in party.PartyMembers)
            {
                Character character = member.Character;
                Console.WriteLine($"{character.Name} [{character.Id}]'s turn");

                // Show moveset
                string action;
                while (true)
                {
                    Console.Write(String.Join(
                        "\n",
                        $"(1). Attack for {character.Atk} damage [{Math.Round(character.CritRate * 100, 2)}% chance of dealing {character.CritMultiplier}x]",
                        $"(2). Use character skill \"{character.Skill}\" [{character.GetSkillActionText()}]",
                        $"What should {character.Name} do? Enter 1 or 2: "
                    ));

                    string enteredAction = (Console.ReadLine() ?? "").Trim();
                    if (enteredAction != "1" && enteredAction != "2")
                    {
                        Console.WriteLine("\nInvalid input. Please enter 1 or 2.");
                        continue;
                    }

                    action = enteredAction;
                    break;
                }

                // TODO: Handle moveset
                // TODO: Damage/Use skill
                // TODO: Handle logic
                // TODO: Update damage done
                // TODO: Redraw
                // TODO: StdOut action
            }
        }

        // Treat as defeated
        if (dm.Hp == 0)
        {
            return;
        }

        // Treat as all dead
    }
}
