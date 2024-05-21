/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestTeams : IUsingContext
{
    [Fact]
    public void CreateTeam()
    {
        Team team = new Team("team1", "desc");
        Db.GetProfile(ProfileId, IncludeFlags.Teams).Teams.Add(team);
        Db.SaveChanges();

        Assert.Equal("team1", team.Name);
        Assert.Equal("desc", team.Description);
    }

    [Fact]
    public void LookupTeamFromProfile()
    {
        Team team = new Team("team2", "desc2");
        Db.GetProfile(ProfileId, IncludeFlags.Teams).Teams.Add(team);
        Db.SaveChanges();

        Team? foundTeam = Db.GetTeams(ProfileId).FirstOrDefault(c => c.Id == team.Id);
        Assert.NotNull(foundTeam);
        Assert.Equal("team2", foundTeam.Name);
        Assert.Equal("desc2", foundTeam.Description);
    }

    [Fact]
    public void LookupTeamFromDb()
    {
        Team team = new Team("team3", "desc3");
        Db.GetProfile(ProfileId, IncludeFlags.Teams).Teams.Add(team);
        Db.SaveChanges();

        Team? foundTeam = Db.Teams.FirstOrDefault(c => c.Id == team.Id);
        Assert.NotNull(foundTeam);
        Assert.Equal("team3", foundTeam.Name);
        Assert.Equal("desc3", foundTeam.Description);
    }
}
