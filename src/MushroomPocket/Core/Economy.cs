/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace MushroomPocket.Core;

public static class Economy
{
    /// <summary>
    /// Last time passive money was auto awarded
    /// </summary
    public static DateTime LastPassiveEarned { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Earn per minute
    /// </summary>
    public static readonly float EarnPerMin = 1;

    /// <summary>
    /// Adds to profile Wallet
    /// </summary>
    public static void AwardMoney(int amount) => AwardMoney((float)amount);

    public static void AwardMoney(float amount)
    {
        using (MushroomContext db = new MushroomContext())
        {
            db.GetProfile().Wallet += amount;
            db.SaveChanges();
        }
    }

    /// <summary>
    /// True only if coins added
    /// </summary>
    public static bool HandlePassiveEarning()
    {
        TimeSpan timePassed = DateTime.UtcNow - LastPassiveEarned;
        if (timePassed.Minutes <= 0)
            return false;

        float toEarn = EarnPerMin * timePassed.Minutes;
        AwardMoney(toEarn);

        // StdOut
        Console.WriteLine($"\n{toEarn} coins earned. ({timePassed.Minutes} minutes passed.)");
        LastPassiveEarned = DateTime.UtcNow;
        return true;
    }
}
