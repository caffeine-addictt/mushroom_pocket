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
    public static void Start(Team team, Dungeon dungeon)
    {
        // Threaded so I can setup some stuff while it shows loading screen in the meantime
        LoadingHandler initialLoadHandler = Loading.Start();
        DungeonMaster dm = new DungeonMaster(dungeon);

        /* int totalDamageDone; */
        /* int totalDamageTaken; */

        Thread.Sleep(3000);
        initialLoadHandler.Dispose();

        // Helper functions
        bool ContinueGame() => !team.Characters.All(c => c.Hp == 0) && dm.Hp != 0;
        Character PickCharacter() => team.Characters.ToList()[new Random().Next(0, team.Characters.Count)];
        void DamageCharacter(Character character, float damage) => character.Hp -= damage;

        // Game loop
        while (ContinueGame())
        {
            // DM moves first
            Frame.DrawFrame(team, dm);
            Console.WriteLine("DM's turn");

            Character target = PickCharacter();
            float damage = RollDamage(dm);
            totalDamageTaken += damage;
            DamageCharacter(target, damage);

            // Redraw
            Frame.DrawFrame(team, dm);
            Console.WriteLine($"DM hits {target.Name} [{target.Id}] with {damage} damage. [{target.Hp}] HP remaining.");


        }

        // Treat as defeated
        if (dm.Hp == 0)
        {
            return;
        }

        // Treat as all dead
    }


    private static float RollDamage(Character c)
        => c.Atk * (c.CritRate * 10 < new Random().Next(11) ? 1 : c.CritMultiplier);
    private static float RollDamage(DungeonMaster dm)
        => dm.Atk * (new Random().Next(11) / 10);
}
