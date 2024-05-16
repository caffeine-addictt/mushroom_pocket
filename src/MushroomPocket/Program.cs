/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;
using MushroomPocket.Core;

namespace MushroomPocket;

class Program
{
    static readonly string InterfaceText = String.Join(
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
            @"(6). Manage my item(s)",
            @"(7). Manage my team(s)",
            @"(8). Manage my profile(s)",
            @"Please only enter [1, 2, 3, 4, 5, 6, 7, 8] or Q to quit: "
        ]
    );

    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine("Usage: MushroomPocket");
            Environment.Exit(1);
        }

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
            // get/create a profile
            ManageProfiles.FirstTimeOrAccess();

            // Proceed on if there is a profile
            if (Constants.CurrentProfileId == null) continue;

            Console.Write(InterfaceText);
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
                    ManageItems.MainEntry();
                    break;

                case "7":
                    ManageTeams.MainEntry();
                    break;

                case "8":
                    ManageProfiles.MainEntry();
                    break;

                case "q":
                    Console.WriteLine("Thanks for playing. Good bye!");
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine(
                        "\nInvalid action. Please only enter [1, 2, 3, 4, 5, 6, 7, 8] or Q to quit."
                    );
                    break;
            }

            // Handle passive
            Economy.HandlePassiveEarning();

            // Insert newline
            Console.WriteLine();
        }
    }
}
