/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Models;

public class Mario : Character
{
    public Mario(float hp, int exp) : base(hp, exp)
    {
        Name = "Mario";
        EvolvedOnly = true;
        Skill = "Combat Skills";
    }
}
