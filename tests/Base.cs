/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace tests;


public class IUsingContext : IDisposable
{
    public MushroomContext Db;
    public string ProfileId;

    protected IUsingContext()
    {
        Db = new MushroomContext();

        // Ensure created
        Db.Database.EnsureCreated();

        // Create new testing profile
        Profile profile = new Profile("testing-" + Guid.NewGuid().ToString());
        ProfileId = profile.Id;

        Db.Profiles.Add(profile);
        Db.SaveChanges();
    }

    public void Dispose()
    {
        // Delete testing profile
        Db.Profiles.Remove(Db.Profiles.First(p => p.Id == ProfileId));
        Db.Dispose();
    }
}
