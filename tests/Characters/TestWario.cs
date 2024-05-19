/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestWario : IUsingContext
{
    [Fact]
    public void CreateWario()
    {
        Wario wario = new Wario(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(wario);
        Db.SaveChanges();

        Assert.Equal(10, wario.Hp);
        Assert.Equal(10, wario.Exp);
        Assert.True(wario.CritRate <= 1);
        Assert.True(wario.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupWarioFromProfile()
    {
        Wario wario = new Wario(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(wario);
        Db.SaveChanges();

        Character? warioChar = Db.GetCharacters(ProfileId).FirstOrDefault(c => c.Id == wario.Id);
        Assert.NotNull(warioChar);
        Assert.Equal(10, warioChar.Hp);
        Assert.Equal(10, warioChar.Exp);
        Assert.True(warioChar.CritRate <= 1);
        Assert.True(warioChar.CritMultiplier >= 1);
    }

    [Fact]
    public void LookupWarioFromDb()
    {
        Wario wario = new Wario(10, 10);
        Db.GetProfile(ProfileId, IncludeFlags.Characters).Characters.Add(wario);
        Db.SaveChanges();

        Character? warioChar = Db.Characters.FirstOrDefault(c => c.Id == wario.Id);
        Assert.NotNull(warioChar);
        Assert.Equal(10, warioChar.Hp);
        Assert.Equal(10, warioChar.Exp);
        Assert.True(warioChar.CritRate <= 1);
        Assert.True(warioChar.CritMultiplier >= 1);
    }
}
