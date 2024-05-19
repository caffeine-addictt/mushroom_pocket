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

        // Game loop
        while (!team.Characters.All(c => c.Hp == 0) && dm.Hp != 0)
        {
            // DM moves first
            Frame.DrawFrame(team, dm);
            Thread.Sleep(1000);
        }

        // Treat as defeated
        if (dm.Hp == 0)
        {
            return;
        }

        // Treat as all dead
    }
}
