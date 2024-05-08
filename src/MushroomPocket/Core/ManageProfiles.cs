/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using Microsoft.EntityFrameworkCore;
using MushroomPocket.Models;
using MushroomPocket.Utils;

namespace MushroomPocket.Core;


static class ManageProfiles
{
    public static readonly string InterfaceText = String.Join(
        "\n",
        [
            @"",
            @"(1). Add a new profile",
            @"(2). Use profile",
            @"(3). Show current profile",
            @"(4). List all profile(s)",
            @"(5). Delete profile(s)",
            @"Please only enter [1, 2, 3, 4, 5] or b to go back: "
        ]
    );

    // Main entry point
    public static void MainEntry()
    {
        Console.Write(InterfaceText);
        switch ((Console.ReadLine() ?? "").ToLower())
        {
            case "1":
                AddProfile();
                break;

            case "2":
                UseProfile();
                break;

            case "3":
                ShowCurrentProfile();
                break;

            case "4":
                ListAllProfiles();
                break;

            case "5":
                Console.WriteLine("Soon");
                break;

            case "b":
                return;

            default:
                Console.WriteLine("\nInvalid input, please only enter [1, 2, 3, 4, 5] or b to go back.");
                break;
        }
    }

    public static void FirstTimeOrAccess()
    {
        if (!String.IsNullOrWhiteSpace(Constants.CurrentProfileId))
            return;

        List<Profile> profiles;
        using (MushroomContext db = new MushroomContext())
            profiles = db.Profiles.ToList();

        // Create if there are no profiles
        if (profiles.Count == 0)
        {
            Console.WriteLine("Welcome to MushroomPocket, please create a new profile.");
            AddProfile();
            Console.WriteLine();
            return;
        }

        // Show interface text
        Console.WriteLine("Welcome to MushroomPocket, please choose your profile.");
        UseProfile();
        Console.WriteLine();
    }

    // Option 7-1: Add profile
    private static void AddProfile()
    {
        // Ask for name
        Console.Write("Please enter profile name: ");
        string name = Console.ReadLine() ?? "";

        if (String.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nProfile name cannot be empty.");
            return;
        }

        // Create
        Profile profile = new Profile(name);
        using (MushroomContext db = new MushroomContext())
        {
            db.Profiles.Add(profile);
            db.SaveChanges();
        }

        // Update constants
        Constants.CurrentProfileId = profile.Id;
        Console.WriteLine(Constants.CurrentProfileId);

        Console.WriteLine("Created profile successfully.");
    }

    private static void UseProfile()
    {
        // Ask for name
        Console.Write("Please enter profile name: ");
        string name = Console.ReadLine() ?? "";

        if (String.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nProfile name cannot be empty.");
            return;
        }

        List<Profile> profiles;
        using (MushroomContext db = new MushroomContext())
            profiles = db.Profiles.ToList();

        Similarity topSuggestion;
        if (!StringUtils.SmartLookUp(name, profiles.Select(p => p.Name), out topSuggestion!))
        {
            Console.WriteLine("\nProfile not found.");
            return;
        }

        if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
        {
            Console.Write(
                $"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: "
            );
            if ((Console.ReadLine() ?? "").ToLower() != "y")
                return;
        }

        Profile profile = profiles.First(p => p.Name == topSuggestion.QualifiedText);
        Constants.CurrentProfileId = profile.Id;
        Console.WriteLine("Now using profile: " + profile.Name);
    }

    // Option 7-3: Show current profile
    private static void ShowCurrentProfile()
    {
        Console.WriteLine(Constants.CurrentProfileId);
        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.Profiles.Where(p => p.Id == Constants.CurrentProfileId).Include(p => p.Characters).Include(p => p.Teams).First()!;
            Console.WriteLine(String.Join("\n", [
                @"-----------------------",
                $"Current profile: {profile.Name}",
                $"Wallet: ${profile.Wallet}",
                $"Team(s): {profile.Teams.Count()}",
                $"Character(s): {profile.Characters.Count()}",
                @"-----------------------",
            ]));
        }
    }

    // Option 7-4: List all profiles
    private static void ListAllProfiles()
    {
        List<Profile> profiles;
        using (MushroomContext db = new MushroomContext())
            profiles = db.Profiles.Include(p => p.Characters).Include(p => p.Teams).ToList();

        if (profiles.Count == 0)
        {
            Console.WriteLine("No profile found.");
            return;
        }

        foreach (Profile profile in profiles)
            Console.WriteLine(String.Join("\n", [
                @"-----------------------",
                $"Profile: {profile.Name}",
                $"Wallet: ${profile.Wallet}",
                $"Team(s): {profile.Teams.Count()}",
                $"Character(s): {profile.Characters.Count()}",
                @"-----------------------",
            ]));
    }
}
