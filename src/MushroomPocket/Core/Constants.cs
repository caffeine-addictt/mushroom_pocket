/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Core;


public struct Dimension
{
    public int X { get; set; }
    public int Y { get; set; }

    public int Width => X;
    public int Height => Y;

    public Dimension(int x, int y)
    {
        X = x;
        Y = y;
    }
}


public static class Constants
{
    public static string? CurrentProfileId { get; set; }

    // Terminal
    public static readonly Dimension TerminalSize = new Dimension(Console.WindowWidth, Console.WindowHeight);
}
