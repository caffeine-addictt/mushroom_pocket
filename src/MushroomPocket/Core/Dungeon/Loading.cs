/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Utils;

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

    /// <summary>
    /// Loading state
    ///
    /// 0 = -
    /// 1 = \
    /// 2 = |
    /// 3 = /
    /// </summary>
    public static int LoadingState = 0;
    public static DateTime LastLoadingStateUpdate = DateTime.UtcNow;

    public static DateTime LastTipUpdate = DateTime.UtcNow;
    private static string[] TipsPool = new string[6] {
        "Check your stats:\nHeal before entering a dungeon",
        "Experiment with skills:\nBuffs can be stacked",
        "Spend your money wisely:\nNever know when you need to heal",
        "Plan your attacks:\nDamage multipliers apply to the final damage calculation",
        "Be prepared:\nYou cannot use items in the dungeon",
        "Manage your team:\nCharacters work best with others",
    };


    private static string GetThrobber()
    {
        if (DateTime.UtcNow - LastLoadingStateUpdate > TimeSpan.FromSeconds(1))
        {
            LoadingState = LoadingState == 3 ? 0 : LoadingState + 1;
            LastLoadingStateUpdate = DateTime.UtcNow;
        }
        return new string[] { "|", "/", "-", "\\" }[LoadingState];
    }

    private static string GetTip()
    {
        if (DateTime.UtcNow - LastTipUpdate > TimeSpan.FromSeconds(30))
        {
            LastTipUpdate = DateTime.UtcNow;
            new Random().Shuffle(TipsPool);
        }
        return TipsPool[0];
    }


    private static void DrawLoading()
    {
        Console.Clear();
        string loadingText = PaddingUtils.CenterAlign(String.Join(
            "\n",
            $"{GetThrobber()}",
            "",
            $"Loading{new string('.', LoadingState)}",
            "",
            "",
            "",
            GetTip()
        ), Constants.TerminalSize.X, "\n");

        // Pad Y-Axis
        Padded padded = PaddingUtils.Pad(loadingText.Split("\n").Count(), Constants.TerminalSize.Y);
        Console.WriteLine(new string('\n', padded.Leading) + $"\n{loadingText}\n" + new string('\n', padded.Trailing));
    }

    private static void LoadingAnimation(int minMs = 10000)
    {
        State = 1;
        DateTime end = DateTime.UtcNow.AddMilliseconds(minMs);

        Console.Clear();
        while (State == 1 || ((end - DateTime.UtcNow) > TimeSpan.Zero))
        {
            Thread.Sleep(250);
            DrawLoading();
        }

        Console.Clear();
    }

    public static LoadingHandler Start()
    {
        Thread loadingThread = new Thread(new ThreadStart(() => LoadingAnimation()));
        loadingThread.Start();

        return new LoadingHandler() { HandlerThread = loadingThread };
    }

}
