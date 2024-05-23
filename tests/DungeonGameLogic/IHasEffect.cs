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
    private bool CheckEqual(IHasEffect e1, IHasEffect e2)
    {
        Assert.NotNull(e1);
        Assert.NotNull(e2);

        Assert.Equal(e1.PlusAtk.Value, e2.PlusAtk.Value);
        Assert.Equal(e1.PlusAtk.RoundsLeft, e2.PlusAtk.RoundsLeft);

        Assert.Equal(e1.PlusCritRate.Value, e2.PlusCritRate.Value);
        Assert.Equal(e1.PlusCritRate.RoundsLeft, e2.PlusCritRate.RoundsLeft);

        Assert.Equal(e1.PlusCritMultiplier.Value, e2.PlusCritMultiplier.Value);
        Assert.Equal(e1.PlusCritMultiplier.RoundsLeft, e2.PlusCritMultiplier.RoundsLeft);

        Assert.Equal(e1.PlusDamageMultiplier.Value, e2.PlusDamageMultiplier.Value);
        Assert.Equal(e1.PlusDamageMultiplier.RoundsLeft, e2.PlusDamageMultiplier.RoundsLeft);

        return true;
    }

    [Fact]
    public void TestMergingDefault()
    {
        IHasEffect e1 = new IHasEffect();
        IHasEffect e2 = new IHasEffect();

        IHasEffect e3 = IHasEffect.MergeEffect(e1, e2);
        Assert.True(CheckEqual(e1, e3));
    }

    [Fact]
    public void TestMergingAtk()
    {
        IHasEffect e1 = new IHasEffect();
        IHasEffect e2 = new IHasEffect();
        e1.PlusAtk.Add(1, 1f);
        e2.PlusAtk.Add(2, 2f);

        IHasEffect e3 = IHasEffect.MergeEffect(e1, e2);
        IHasEffect expected = new IHasEffect();
        expected.PlusAtk.Add(2, 3f);

        Assert.True(CheckEqual(expected, e3));
    }

    [Fact]
    public void TestingMergingCritRate()
    {
        IHasEffect e1 = new IHasEffect();
        IHasEffect e2 = new IHasEffect();
        e1.PlusCritRate.Add(1, 1f);
        e2.PlusCritRate.Add(2, 2f);

        IHasEffect e3 = IHasEffect.MergeEffect(e1, e2);
        IHasEffect expected = new IHasEffect();
        expected.PlusCritRate.Add(2, 3f);

        Assert.True(CheckEqual(expected, e3));
    }

    [Fact]
    public void TestMergingCritMultiplier()
    {
        IHasEffect e1 = new IHasEffect();
        IHasEffect e2 = new IHasEffect();
        e1.PlusCritMultiplier.Add(1, 1f);
        e2.PlusCritMultiplier.Add(2, 2f);

        IHasEffect e3 = IHasEffect.MergeEffect(e1, e2);
        IHasEffect expected = new IHasEffect();
        expected.PlusCritMultiplier.Add(2, 3f);

        Assert.True(CheckEqual(expected, e3));
    }
}
