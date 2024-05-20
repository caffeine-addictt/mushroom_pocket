/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Core.DungeonGameLogic;

namespace tests;


public class TestIHasEffect
{
    [Fact]
    public void TestMergingDefault()
    {
        IHasEffect e1 = new IHasEffect();
        IHasEffect e2 = new IHasEffect();

        IHasEffect e3 = IHasEffect.MergeEffect(e1, e2);
        Assert.Equal(e1.PlusAtk.Value, e3.PlusAtk.Value);
        Assert.Equal(e1.PlusDamageMultiplier.Value, e3.PlusDamageMultiplier.Value);
    }
}
