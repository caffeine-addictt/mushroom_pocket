/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Models;


public class ExpPotion : Item
{
    public static new int Price = 5;
    public static new string Description = "Induces strangely vivid dreams. They claim not to remember anything, but they seem.. stronger...";
    public ExpPotion() : base("ExpPotion") { }

    public override string SuccessEcho()
        => $"Grade {Grade} Exp Potion increased experience by {ExpToAdd()}!";

    /// <summary>
    /// Calculates the amount of xp to give
    /// </summary>
    public int ExpToAdd()
        => (int)Math.Floor((decimal)(200 / Grade));

    /// <summary>
    /// Increases Exp by (200/Grade)
    /// </summary>
    public override void Use(Character c)
    {
        c.Exp += ExpToAdd();
    }
}
