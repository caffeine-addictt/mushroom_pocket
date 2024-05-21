/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class TestLookup : IUsingContext
{
    // Character.Teams
    [Fact]
    public void CharacterTeamsWithCompoundFlag()
    {
        Profile profile = Db.GetProfile(ProfileId, IncludeFlags.Teams | IncludeFlags.Characters);

        Character character = Character.From("Daisy", 10, 10);
        profile.Characters.Add(character);

        Team team = new Team("team1", "desc", new HashSet<Character>() { character });
        profile.Teams.Add(team);

        Db.SaveChanges();

        // Check
        Character? foundCharacter = Db.GetCharacters(ProfileId, IncludeFlags.CharacterTeams).FirstOrDefault(c => c.Id == character.Id);
        Assert.NotNull(foundCharacter);
        Assert.Single<Team>(foundCharacter.Teams);
    }

    // Character.Teams
    [Fact]
    public void CharacterTeamsWithInferredFlag()
    {
        Profile profile = Db.GetProfile(ProfileId, IncludeFlags.Teams | IncludeFlags.Characters);

        Character character = Character.From("Daisy", 10, 10);
        profile.Characters.Add(character);

        Team team = new Team("team2", "desc", new HashSet<Character>() { character });
        profile.Teams.Add(team);

        Db.SaveChanges();

        // Check
        Character? foundCharacter = Db.GetCharacters(ProfileId, IncludeFlags.Teams).FirstOrDefault(c => c.Id == character.Id);
        Assert.NotNull(foundCharacter);
        Assert.Single<Team>(foundCharacter.Teams);
    }


    // Teams.Characters
    [Fact]
    public void TeamCharactersWithCompoundFlag()
    {
        Profile profile = Db.GetProfile(ProfileId, IncludeFlags.Teams | IncludeFlags.Characters);

        Character character = Character.From("Daisy", 10, 10);
        profile.Characters.Add(character);

        Team team = new Team("team1", "desc", new HashSet<Character>() { character });
        profile.Teams.Add(team);

        Db.SaveChanges();

        // Check
        Team? foundTeam = Db.GetTeams(ProfileId, IncludeFlags.TeamCharacters).FirstOrDefault(c => c.Id == team.Id);
        Assert.NotNull(foundTeam);
        Assert.Single<Character>(foundTeam.Characters);
    }

    // Teams.Characters
    [Fact]
    public void TeamCharactersWithInferredFlag()
    {
        Profile profile = Db.GetProfile(ProfileId, IncludeFlags.Teams | IncludeFlags.Characters);

        Character character = Character.From("Daisy", 10, 10);
        profile.Characters.Add(character);

        Team team = new Team("team2", "desc", new HashSet<Character>() { character });
        profile.Teams.Add(team);

        Db.SaveChanges();

        // Check
        Team? foundTeam = Db.GetTeams(ProfileId, IncludeFlags.Characters).FirstOrDefault(c => c.Id == team.Id);
        Assert.NotNull(foundTeam);
        Assert.Single<Character>(foundTeam.Characters);
    }
}
