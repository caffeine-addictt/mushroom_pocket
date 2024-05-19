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
        LoadingHandler initialLoadHandler = Loading.Start();
        DungeonMaster dm = new DungeonMaster(dungeon);

        Thread.Sleep(3000);
        initialLoadHandler.Dispose();
    }
}
