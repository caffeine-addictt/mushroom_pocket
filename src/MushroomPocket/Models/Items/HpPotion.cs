/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Models;


public class HpPotion : Item
{
    public HpPotion() : base("HpPotion", "Poisonious mushrooms, Cyanide and some weird cloud. Forget healing - they lived?!", 3) { }

    public override string SuccessEcho()
        => $"Grade {Grade} Hp Potion healed {HpToHeal()}hp!";

    /// <summary>
    /// Calculates the amount of hp to heal
    /// </summary>
    public float HpToHeal()
        => (float)Math.Floor((decimal)(100 / Grade));

    /// <summary>
    /// Heals (100/Grade) hp
    /// </summary>
    public override void Use(Character c)
    {
        c.Hp += HpToHeal();
    }
}
