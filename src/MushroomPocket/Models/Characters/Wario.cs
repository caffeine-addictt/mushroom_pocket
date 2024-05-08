/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Models;

public class Wario : Character
{
    public Wario(float hp, int exp) : base(hp, exp)
    {
        Name = "Wario";
        Skill = "Strength";
        EvolvedOnly = false;
    }
}
