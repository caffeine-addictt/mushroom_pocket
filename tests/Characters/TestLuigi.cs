/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestLuigi : IUsingContext
{
    [Fact]
    public void CreateLuigi()
    {
        Luigi luigi = new Luigi(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(luigi);
        Db.SaveChanges();

        Assert.Equal(10, luigi.Hp);
        Assert.Equal(10, luigi.Exp);
        Assert.True(luigi.CritRate <= 1);
        Assert.True(luigi.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupLuigiFromProfile()
    {
        Luigi luigi = new Luigi(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(luigi);
        Db.SaveChanges();

        Character? luigiChar = Db.GetCharacters(ProfileId).FirstOrDefault(c => c.Id == luigi.Id);
        Assert.NotNull(luigiChar);
        Assert.Equal(10, luigiChar.Hp);
        Assert.Equal(10, luigiChar.Exp);
        Assert.True(luigiChar.CritRate <= 1);
        Assert.True(luigiChar.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupLuigiFromDb()
    {
        Luigi luigi = new Luigi(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(luigi);
        Db.SaveChanges();

        Character? luigiChar = Db.Characters.FirstOrDefault(c => c.Id == luigi.Id);
        Assert.NotNull(luigiChar);
        Assert.Equal(10, luigiChar.Hp);
        Assert.Equal(10, luigiChar.Exp);
        Assert.True(luigiChar.CritRate <= 1);
        Assert.True(luigiChar.CritMultiplier >= 1);
    }
}
