/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Models;

public class Daisy : Character
{
    public Daisy(float hp, int exp) : base(hp, exp)
    {
        Name = "Daisy";
        EvolvedOnly = false;
        Skill = "Leadership";
    }
}
