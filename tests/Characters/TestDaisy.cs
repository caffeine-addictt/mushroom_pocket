/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestDaisy : IUsingContext
{
    [Fact]
    public void CreateDaisy()
    {
        Daisy daisy = new Daisy(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(daisy);
        Db.SaveChanges();

        Assert.Equal(10, daisy.Hp);
        Assert.Equal(10, daisy.Exp);
        Assert.True(daisy.CritRate <= 1);
        Assert.True(daisy.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupDaisyFromProfile()
    {
        Daisy daisy = new Daisy(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(daisy);
        Db.SaveChanges();

        Character? daisyChar = Db.GetCharacters(ProfileId).FirstOrDefault(c => c.Id == daisy.Id);
        Assert.NotNull(daisyChar);
        Assert.Equal(10, daisyChar.Hp);
        Assert.Equal(10, daisyChar.Exp);
        Assert.True(daisyChar.CritRate <= 1);
        Assert.True(daisyChar.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupDaisyFromDb()
    {
        Daisy daisy = new Daisy(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(daisy);
        Db.SaveChanges();

        Character? daisyChar = Db.Characters.FirstOrDefault(c => c.Id == daisy.Id);
        Assert.NotNull(daisyChar);
        Assert.Equal(10, daisyChar.Hp);
        Assert.Equal(10, daisyChar.Exp);
        Assert.True(daisyChar.CritRate <= 1);
        Assert.True(daisyChar.CritMultiplier >= 1);
    }
}
