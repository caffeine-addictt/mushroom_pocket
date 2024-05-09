/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using System.Reflection.Metadata;
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
                DeleteProfiles();
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

    // Option 7-5: Delete profiles
    private static void DeleteProfiles()
    {
        Console.Write(String.Join(
            "\n",
            @"",
            @"(1). Delete profile from ID",
            @"(2). Delete profile from Name",
            @"(3). Delete profile from pattern",
            @"(4). Delete all profiles",
            @"Please only enter [1, 2, 3, 4] or b to go back: "
        ));

        switch ((Console.ReadLine() ?? "").ToLower()) {
            case "1":
                DeleteProfileFromID();
                break;

            case "2":
                DeleteProfileFromName();
                break;

            case "3":
                DeleteProfileFromPattern();
                break;

            case "4":
                DeleteAllProfiles();
                break;

            case "b":
                return;
            
            default:
                Console.WriteLine("\nInvalid input. Please only enter [1, 2, 3, 4] or b to go back.");
                break;
        }
    }

    // Option 7-5-1: Delete from ID
    private static void DeleteProfileFromID()
    {
        // Ask for ID
        Console.Write("Enter Profile ID: ");
        string id = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("\nProfile ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Profile> profiles = db.Profiles.ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(id, profiles.Select(p => p.Id.ToString()), out topSuggestion!))
            {
                Console.WriteLine("\nProfile not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Profile profile = profiles.First(p => p.Id.ToString() == topSuggestion.QualifiedText);

            // Check if profile is in use
            bool inUse = profile.Id == Constants.CurrentProfileId;
            if (inUse)
            {
                Console.Write("This profile is currently in use. Are you sure you want to delete it? [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                Constants.CurrentProfileId = null;
            }

            db.Profiles.Remove(profile);
            db.SaveChanges();

            Console.WriteLine($"Delete profile: {profile.Name}");
        }
    }

    // Option 7-5-2: Delete from Name
    private static void DeleteProfileFromName()
    {
        // Ask for name
        Console.Write("Enter Profile Name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nProfile Name cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Profile> profiles = db.Profiles.ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(name, profiles.Select(p => p.Name), out topSuggestion!))
            {
                Console.WriteLine("\nProfile not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Profile profile = profiles.First(p => p.Name == topSuggestion.QualifiedText);

            // Check if profile is in use
            bool inUse = profile.Id == Constants.CurrentProfileId;
            if (inUse)
            {
                Console.Write("This profile is currently in use. Are you sure you want to delete it? [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                Constants.CurrentProfileId = null;
            }

            db.Profiles.Remove(profile);
            db.SaveChanges();

            Console.WriteLine($"Delete profile: {profile.Name}");
        }
    }

    // Option 7-5-3: Delete from pattern
    private static void DeleteProfileFromPattern()
    {
        // Ask for pattern
        Console.Write("Enter Profile Name or ID: ");
        string pattern = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(pattern))
        {
            Console.WriteLine("\nProfile name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Profile> profiles = db.GetProfiles()
                .Where(p =>
                    p.Id.ToLower().StartsWith(pattern)
                    || p.Name.ToLower().StartsWith(pattern)
                )
                .ToList();

            if (profiles.Count == 0)
            {
                Console.WriteLine("\nNo profile(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {profiles.Count} profile(s)? [Y/N] or L to list the affected profiles: ");            
            switch ((Console.ReadLine() ?? "").ToLower()) {
                case "l":
                    foreach (Profile profile in profiles)
                        Console.WriteLine(String.Join(
                            "\n",
                            @"-----------------------",
                            $"ID: {profile.Id}",
                            $"Name: {profile.Name}",
                            $"Wallet: ${profile.Wallet}",
                            $"Team(s): {profile.Teams.Count()}",
                            $"Character(s): {profile.Characters.Count()}",
                            @"-----------------------"
                        ));

                    // Final confirmation
                    Console.Write("Are you sure you want to delete these profiles? [Y/N]: ");
                    if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                    break;
                case "y":
                    break;

                default:
                    return;
            }

            // Check if profile is in use
            bool inUse = profiles.Any(p => p.Id == Constants.CurrentProfileId);
            if (inUse)
            {
                Console.Write($"A profile currently in use will be deleted. Are you sure you want to delete it? [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                Constants.CurrentProfileId = null;
            }

            // Delete
            db.Profiles.RemoveRange(profiles);
            Console.WriteLine($"Delete {profiles.Count} profile(s).");
        }
    }

    // Option 7-5-4: Delete all profiles
    private static void DeleteAllProfiles()
    {
        using (MushroomContext db = new MushroomContext())
        {
            Console.Write($"Are you sure you want to delete {db.Profiles.Count()} profile(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            db.Profiles.RemoveRange(db.Profiles.ToList());
            db.SaveChanges();

            Constants.CurrentProfileId = null;
            Console.WriteLine("All profiles deleted.");
        }
    }
}
