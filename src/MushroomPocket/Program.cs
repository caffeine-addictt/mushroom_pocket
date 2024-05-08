/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;
using MushroomPocket.Utils;
using MushroomPocket.Core;

namespace MushroomPocket;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine("Usage: MushroomPocket");
            Environment.Exit(1);
        }

        // Ensure Save Dir
        SaveUtils.EnsureSaveDir();

        // Ensure DB exists
        using (MushroomContext db = new MushroomContext())
        {
            db.Database.EnsureCreated();
        }

        // MushroomMaster criteria list for checking character transformation availability.
        List<MushroomMaster> mushroomMasters = new List<MushroomMaster>()
        {
            new MushroomMaster("Daisy", 2, "Peach"),
            new MushroomMaster("Wario", 3, "Mario"),
            new MushroomMaster("Waluigi", 1, "Luigi"),
        };

        // Validate MushroomMaster list
        List<string> violations = new List<string>();
        foreach (MushroomMaster m in mushroomMasters)
        {
            if (!Character.IsValidName(m.Name))
                violations.Add(
                    $"Attribute name {m.Name} in new MushroomMaster(\"{m.Name}\", \"{m.NoToTransform}\", \"{m.TransformTo}\") is invalid."
                );
            if (!Character.IsValidName(m.TransformTo))
                violations.Add(
                    $"Attribute transformTo {m.TransformTo} in new MushroomMaster(\"{m.Name}\", \"{m.NoToTransform}\", \"{m.TransformTo}\") is invalid."
                );
        }

        if (violations.Count > 0)
        {
            Console.WriteLine(
                String.Join(
                    "\n",
                    new List<string>()
                    {
                        "Validating mushroomMasters failed. Please fix the following errors:",
                    }.Concat(violations)
                )
            );
            Environment.Exit(1);
        }

        // Main event loop.
        while (true)
        {
            // Ask for action.
            Console.Write(
                String.Join(
                    "\n",
                    [
                        @"********************************",
                        @"Welcome to Mushroom Pocket App",
                        @"********************************",
                        @"(1). Add Mushroom's character to my pocket",
                        @"(2). List character(s) in my pocket",
                        @"(3). Check if I can transform my characters",
                        @"(4). Transform my character(s)",
                        @"(5). Delete character(s) from my pocket",
                        @"(6). Manage my teams",
                        @"(7). Manage my saves",
                        @"Please only enter [1, 2, 3, 4, 5, 6, 7] or Q to quit: "
                    ]
                )
            );

            switch ((Console.ReadLine() ?? "").ToLower())
            {
                case "1":
                    ManageCharacters.AddCharacter();
                    break;

                case "2":
                    ManageCharacters.ListCharacters();
                    break;

                case "3":
                    ManageCharacters.CheckTransformation(mushroomMasters);
                    break;

                case "4":
                    ManageCharacters.TransformCharacters(mushroomMasters);
                    break;

                case "5":
                    ManageCharacters.DeleteCharacters();
                    break;

                case "6":
                    ManageTeams.MainEntry();
                    break;

                case "7":
                    ManageSaves();
                    break;

                case "q":
                    Console.WriteLine("Thanks for playing. Good bye!");
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine(
                        "\nInvalid action. Please only enter [1, 2, 3, 4] or Q to quit."
                    );
                    break;
            }

            // Insert newline
            Console.WriteLine();
        }
    }

    // Option 7: Manage saves
    private static void ManageSaves()
    {
        Console.Write(
            String.Join(
                "\n",
                [
                    @"",
                    @"(1). Save progress",
                    @"(2). Load progress",
                    @"(3). List saves",
                    @"(4). Delete save(s)",
                    @"Please only enter [1, 2, 3, 4] or b to go back: ",
                ]
            )
        );

        switch ((Console.ReadLine() ?? "").ToLower())
        {
            case "1":
                SaveProgress();
                break;

            case "2":
                LoadProgress();
                break;

            case "3":
                ListSaves();
                break;

            case "4":
                DeleteSaves();
                break;

            case "b":
                return;

            default:
                Console.WriteLine(
                    "\nInvalid input. Please only enter [1, 2, 3, 4] or b to go back."
                );
                break;
        }
    }

    // Option 7-1: Save progress
    private static void SaveProgress()
    {
        // Get name
        Console.Write("Enter save name: ");
        string saveName = Console.ReadLine() ?? "";

        if (String.IsNullOrWhiteSpace(saveName))
        {
            Console.WriteLine("\nSave name cannot be empty.");
            return;
        }

        SaveUtils.CreateSaveFile(saveName);
        Console.WriteLine($"Save {saveName} has been created.");
    }

    // Option 7-2: Load progress
    private static void LoadProgress()
    {
        // Get name
        Console.Write("Enter save name: ");
        string saveName = (Console.ReadLine() ?? "").Trim();

        Similarity topSuggestion;
        if (!StringUtils.SmartLookUp(saveName, SaveUtils.GetSaveNames(), out topSuggestion!))
        {
            Console.WriteLine("\nSave name not found.");
            return;
        }

        if (
            StringUtils.Clean(saveName, true)
            != StringUtils.Clean(topSuggestion.QualifiedText, true)
        )
        {
            Console.Write($"\nDid you mean {topSuggestion.QualifiedText}? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y")
                return;
        }

        SaveUtils.UseSafeFile(SaveUtils.GetFilePathFromName(topSuggestion.QualifiedText));
        Console.WriteLine(
            $"Save file {saveName} has been loaded, please restart the program to see the changes."
        );
        Environment.Exit(0); // Exit because replacing db is not supported by EF8 :"D
    }

    // Option 7-3: List saves
    private static void ListSaves()
    {
        string[] saves = SaveUtils.GetSaveNames();
        if (saves.Length == 0)
        {
            Console.WriteLine("\nNo save(s) to list!");
            return;
        }

        Console.WriteLine(String.Join("\n", saves));
    }

    // Option 7-4: Delete save(s)
    private static void DeleteSaves()
    {
        // Get name
        Console.Write("Enter save name [* for all]: ");
        string delPattern = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(delPattern))
        {
            Console.WriteLine("\nSave name cannot be empty.");
            return;
        }

        if (delPattern == "*")
        {
            SaveUtils.DeleteSaveFiles(SaveUtils.ListSaveFiles());
            Console.WriteLine("All save files have been deleted.");
            return;
        }

        string[] saveFiles = SaveUtils
            .ListSaveFiles()
            .Where(s => s.ToLower().StartsWith(delPattern.ToLower()))
            .ToArray();
        if (saveFiles.Length == 0)
        {
            Console.WriteLine("\nNo save file(s) found. Nothing to delete.");
            return;
        }

        // Check for confirmation
        Console.Write($"Are you sure you want to delete {saveFiles.Length} save file(s)? [Y/N]: ");
        if ((Console.ReadLine() ?? "").ToLower() != "y")
            return;

        SaveUtils.DeleteSaveFiles(saveFiles);
        Console.WriteLine($"{saveFiles.Length} save file(s) have been deleted.");
    }
}
