/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using System.Runtime.InteropServices;

namespace MushroomPocket.Core.DungeonGameLogic;


public struct LoadingHandler : IDisposable
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


    private static void DrawLoading(IEnumerable<int> yHeight)
    {
        Console.Clear();

        List<List<string>> screen = new List<List<string>>();
        foreach (int y in yHeight)
            screen.Add((new Random()).GetItems<string>(CollectionsMarshal.AsSpan<string>(CharSet), y).ToList());

        int maxY = yHeight.Max();
        int currY = 0;

        while (currY < maxY)
        {
            foreach (List<string> col in screen)
                Console.Write(currY < col.Count ? col[currY] : " ");
            Console.WriteLine();
            currY++;
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
                yHeight[i] = Math.Clamp(yHeight[i] + new Random().Next(1, 5), 0, Constants.TerminalSize.Y);
            }

            // Delay (ms)
            Thread.Sleep(100);

            // Draw
            DrawLoading(yHeight);
        }

        // Animate waiting
        while (State == 1)
        {
            Thread.Sleep(200);
            DrawLoading(yHeight);
        }

        // Animate away
        while (yHeight.Any(i => i != 0))
        {
            // Decrement Y 1-2 each time
            for (int i = 0; i < Constants.TerminalSize.X; i++)
            {
                yHeight[i] = Math.Clamp(yHeight[i] - new Random().Next(1, 5), 0, Constants.TerminalSize.Y);
            }

            // Delay (ms)
            Thread.Sleep(100);

            // Draw
            DrawLoading(yHeight);
        }
    }

    public static LoadingHandler Start()
    {
        Thread loadingThread = new Thread(new ThreadStart(LoadingAnimation));
        loadingThread.Start();

        return new LoadingHandler() { HandlerThread = loadingThread };
    }

}
