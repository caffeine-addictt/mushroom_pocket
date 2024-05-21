/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestMario : IUsingContext
{
    [Fact]
    public void CreateMario()
    {
        Mario mario = new Mario(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(mario);
        Db.SaveChanges();

        Assert.Equal(10, mario.Hp);
        Assert.Equal(10, mario.Exp);
        Assert.True(mario.CritRate <= 1);
        Assert.True(mario.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupMarioFromProfile()
    {
        Mario mario = new Mario(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(mario);
        Db.SaveChanges();

        Character? marioChar = Db.GetCharacters(ProfileId).FirstOrDefault(c => c.Id == mario.Id);
        Assert.NotNull(marioChar);
        Assert.Equal(10, marioChar.Hp);
        Assert.Equal(10, marioChar.Exp);
        Assert.True(marioChar.CritRate <= 1);
        Assert.True(marioChar.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupMarioFromDb()
    {
        Mario mario = new Mario(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(mario);
        Db.SaveChanges();

        Character? marioChar = Db.Characters.FirstOrDefault(c => c.Id == mario.Id);
        Assert.NotNull(marioChar);
        Assert.Equal(10, marioChar.Hp);
        Assert.Equal(10, marioChar.Exp);
        Assert.True(marioChar.CritRate <= 1);
        Assert.True(marioChar.CritMultiplier >= 1);
    }
}
