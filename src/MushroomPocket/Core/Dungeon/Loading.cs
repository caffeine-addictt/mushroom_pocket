/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using System.Runtime.InteropServices;

namespace MushroomPocket.Core.DungeonGameLogic;


public struct LoadingHandler
{
    public Thread? HandlerThread;

    public void Dispose()
    {
        if (HandlerThread == null) throw new NullReferenceException("HandlerThread is null");
        Loading.State = 0;
        HandlerThread.Join();
        HandlerThread = null;
    }
}

public static class Loading
{
    /// <summary>0 - stop, 1 - loading</summary>
    public static byte State = 0;
    private static readonly List<string> CharSet = new List<string>() {
        "0",
        "c",
        "C",
        "(",
        ")",
        "o",
        "O",
        "x",
        "X",
        "1",
        "i",
    };


    /// <summary><paramref name="topFill"/> means to bottom align</summary>
    private static void DrawLoading(IEnumerable<int> yHeight, bool topFill = false)
    {
        List<List<string>> screen = new List<List<string>>();
        foreach (int y in yHeight)
            screen.Add((new Random()).GetItems<string>(CollectionsMarshal.AsSpan<string>(CharSet), y).ToList());

        // Handle top fill
        if (topFill)
        {
            for (int row = 0; row < screen.Count; row++)
            {
                for (int i = 0; i < (Constants.TerminalSize.Y - screen[row].Count); i++)
                    screen[row].Prepend(" ");
            }
        }

        for (int row = 0; row < screen.Count; row++)
        {
            foreach (List<string> col in screen)
                Console.Write(col[row]);
            Console.WriteLine();
        }
    }

    private static void LoadingAnimation()
    {
        State = 1;
        Console.Clear();
        int[] yHeight = new int[Constants.TerminalSize.X];

        // Prefill 0
        for (int i = 0; i < Constants.TerminalSize.X; i++)
        {
            yHeight[i] = 0;
        }

        // Animate top-down
        while (yHeight.Any(i => i != Constants.TerminalSize.Y))
        {
            // Increment Y 1-2 each time
            for (int i = 0; i < Constants.TerminalSize.X; i++)
            {
                yHeight[i] += Math.Clamp(new Random().Next(1, 3), 0, Constants.TerminalSize.Y);
            }

            // Draw
            DrawLoading(yHeight);

            // Delay (ms)
            Thread.Sleep(750);
        }

        // Animate waiting
        while (State == 1)
        {
            DrawLoading(yHeight);
            Thread.Sleep(750);
        }

        // Animate away
        while (yHeight.Any(i => i != 0))
        {
            // Decrement Y 1-2 each time
            for (int i = 0; i < Constants.TerminalSize.X; i++)
            {
                yHeight[i] -= Math.Clamp(new Random().Next(1, 3), 0, Constants.TerminalSize.Y);
            }

            // Draw
            DrawLoading(yHeight);

            // Delay (ms)
            Thread.Sleep(750);
        }
    }

    public static LoadingHandler Start()
        => new LoadingHandler()
        {
            HandlerThread = new Thread(new ThreadStart(LoadingAnimation))
        };

}
