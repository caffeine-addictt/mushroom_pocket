/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestPeach : IUsingContext
{
    [Fact]
    public void CreatePeach()
    {
        Peach peach = new Peach(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(peach);
        Db.SaveChanges();

        Assert.Equal(10, peach.Hp);
        Assert.Equal(10, peach.Exp);
        Assert.True(peach.CritRate <= 1);
        Assert.True(peach.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupPeachFromProfile()
    {
        Peach peach = new Peach(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(peach);
        Db.SaveChanges();

        Character? peachChar = Db.GetCharacters(ProfileId).FirstOrDefault(c => c.Id == peach.Id);
        Assert.NotNull(peachChar);
        Assert.Equal(10, peachChar.Hp);
        Assert.Equal(10, peachChar.Exp);
        Assert.True(peachChar.CritRate <= 1);
        Assert.True(peachChar.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupPeachFromDb()
    {
        Peach peach = new Peach(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(peach);
        Db.SaveChanges();

        Character? peachChar = Db.Characters.FirstOrDefault(c => c.Id == peach.Id);
        Assert.NotNull(peachChar);
        Assert.Equal(10, peachChar.Hp);
        Assert.Equal(10, peachChar.Exp);
        Assert.True(peachChar.CritRate <= 1);
        Assert.True(peachChar.CritMultiplier >= 1);
    }
}
