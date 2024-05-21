/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestWaluigi : IUsingContext
{
    [Fact]
    public void CreateWaluigi()
    {
        Waluigi waluigi = new Waluigi(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(waluigi);
        Db.SaveChanges();

        Assert.Equal(10, waluigi.Hp);
        Assert.Equal(10, waluigi.Exp);
        Assert.True(waluigi.CritRate <= 1);
        Assert.True(waluigi.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupWaluigiFromProfile()
    {
        Waluigi waluigi = new Waluigi(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(waluigi);
        Db.SaveChanges();

        Character? waluigiChar = Db.GetCharacters(ProfileId).FirstOrDefault(c => c.Id == waluigi.Id);
        Assert.NotNull(waluigiChar);
        Assert.Equal(10, waluigiChar.Hp);
        Assert.Equal(10, waluigiChar.Exp);
        Assert.True(waluigiChar.CritRate <= 1);
        Assert.True(waluigiChar.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupWaluigiFromDb()
    {
        Waluigi waluigi = new Waluigi(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(waluigi);
        Db.SaveChanges();

        Character? waluigiChar = Db.Characters.FirstOrDefault(c => c.Id == waluigi.Id);
        Assert.NotNull(waluigiChar);
        Assert.Equal(10, waluigiChar.Hp);
        Assert.Equal(10, waluigiChar.Exp);
        Assert.True(waluigiChar.CritRate <= 1);
        Assert.True(waluigiChar.CritMultiplier >= 1);
    }
}
