/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Core.DungeonGameLogic;

namespace tests;


public class TestEffect
{
    [Fact]
    public void TestDecrement()
    {
        Effect<float> e = new Effect<float>(2, 2f);
        e.Decrement();
        Assert.Equal(1, e.RoundsLeft);
        Assert.Equal(2f, e.Value);
    }

    [Fact]
    public void TestDefaultOnZero()
    {
        Effect<float> e = new Effect<float>(2, 0f);
        e.Value = 2f;
        e.Decrement();
        e.Decrement();

        Assert.Equal(0, e.RoundsLeft);
        Assert.Equal(0f, e.Value);
    }
}
