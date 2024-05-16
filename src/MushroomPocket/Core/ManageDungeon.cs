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
}
