/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;
using MushroomPocket.Utils;

namespace MushroomPocket.Core;


public static class ManageItems
{
    public static readonly string InterfaceText = String.Join(
        "\n",
        @"",
        @"(1). Open shop",
        @"(2). Buy item(s)",
        @"(3). Use item(s)",
        @"(4). List all item(s)",
        @"(5). Delete items",
        @"Please only enter [1, 2, 3, 4, 5] or b to go back: "
    );


    // Echo item
    private static void EchoItem(Item i)
        => Console.WriteLine(String.Join(
            "\n",
            @"-----------------------",
            $"ID: {i.Id}",
            $"Name: {i.Name}",
            $"Grade: {i.Grade}",
            $"Description: {i.GetEffectDescription()}",
            @"-----------------------"
        ));


    // Main Entry
    public static void MainEntry()
    {
        Console.Write(InterfaceText);
        switch ((Console.ReadLine() ?? "").ToLower())
        {
            case "1":
                ListShop();
                break;

            case "2":
                BuyItem();
                break;

            case "3":
                UseItem();
                break;

            case "4":
                ListAllItems();
                break;

            case "5":
                DeleteItems();
                break;

            case "b":
                return;

            default:
                Console.WriteLine("\nInvalid input. Please only enter [1, 2, 3, 4, 5] or b to go back.");
                return;
        }
    }

    // Option 8-1: List items in shop
    private static void ListShop()
        => Console.WriteLine(String.Join(
            "\n",
            @"-----------------------",
            @"Name: Exp Potion",
            $"Price: {ExpPotion.GetPrice("ExpPotion")}",
            $"Description: {ExpPotion.GetDescription("ExpPotion")}",
            @"-----------------------",
            @"-----------------------",
            @"Name: Hp Potion",
            $"Price: {HpPotion.GetPrice("HpPotion")}",
            $"Description: {HpPotion.GetDescription("HpPotion")}",
            @"-----------------------"
        ));

    // Option 8-2: Buy item
    private static void BuyItem()
    {
        Console.Write("Please enter the item name you want to buy [ExpPotion, HpPotion]: ");

        Similarity topSuggestion;
        if (!StringUtils.SmartLookUp((Console.ReadLine() ?? "").ToLower(), ["ExpPotion", "HpPotion"], out topSuggestion!))
        {
            Console.WriteLine("\nInvalid input. Please only enter [ExpPotion, HpPotion].");
            return;
        }

        if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
        {
            Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(IncludeFlags.Items);

            int maxCount = (int)Math.Floor(profile.Wallet / Item.GetPrice(topSuggestion.QualifiedText));
            if (maxCount == 0)
            {
                Console.WriteLine("You don't have enough money. Please check your wallet.");
                return;
            }

            // Count
            Console.Write($"How many would you like to buy? [Max: {maxCount}]: ");
            int count;
            if (!int.TryParse((Console.ReadLine() ?? "").Trim(), out count))
            {
                Console.WriteLine($"\nInvalid input. Please only enter an integer up to {maxCount}.");
                return;
            }

            if (count <= 0 || count > maxCount)
            {
                Console.WriteLine($"\nInvalid input. Please only enter an integer up to {maxCount}.");
                return;
            }

            int cost = (int)Item.GetPrice(topSuggestion.QualifiedText) * count;
            profile.Wallet -= cost;

            switch (topSuggestion.QualifiedText)
            {
                case "ExpPotion":
                    for (int i = 0; i < count; i++)
                        profile.Items.Add(new ExpPotion());
                    break;

                case "HpPotion":
                    for (int i = 0; i < count; i++)
                        profile.Items.Add(new HpPotion());
                    break;
            }

            Console.WriteLine($"\nYou have bought {count} {topSuggestion.QualifiedText} for ${cost}.");
            Console.WriteLine($"[${profile.Wallet} left.]");
            db.SaveChanges();
        }
    }

    // Option 8-3: Use items
    private static void UseItem()
    {
        Console.Write("Please enter the item name you want to use [ExpPotion, HpPotion]: ");
        Similarity topSuggestion;

        if (!StringUtils.SmartLookUp((Console.ReadLine() ?? "").ToLower(), ["ExpPotion", "HpPotion"], out topSuggestion!))
        {
            Console.WriteLine("\nInvalid input. Please only enter [ExpPotion, HpPotion].");
            return;
        }

        if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
        {
            Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Item> items = db.GetItems().Where(i => i.Name == topSuggestion.QualifiedText).ToList();

            if (items.Count == 0)
            {
                Console.WriteLine("\nYou do not have this item.");
                return;
            }

            Console.WriteLine();

            int[] grade = new int[4] { 0, 0, 0, 0 };
            foreach (Item i in items)
                grade[i.Grade]++;

            // Map input option to grade index
            List<int> optionMap = new List<int>();
            for (int i = 0; i < grade.Length; i++)
            {
                if (grade[i] > 0)
                {
                    optionMap.Add(i);
                    Console.WriteLine($"({optionMap.Count}). [x{grade[i]}] Grade {i} item(s): {Item.GetEffectDescription(topSuggestion.QualifiedText, i)}");
                }
            }

            Console.Write($"Enter your choice [{(optionMap.Count > 1 ? $"1 .. {optionMap.Count}" : "1")}]: ");
            int choice;
            if (!int.TryParse((Console.ReadLine() ?? "").Trim(), out choice))
            {
                Console.WriteLine("\nInvalid input. Please only enter a number.");
                return;
            }

            if (choice <= 0 || choice > optionMap.Count)
            {
                Console.WriteLine("\nInvalid input. Please only enter a number between 1 and {optionMap.Length}.");
                return;
            }

            // Convert to index
            choice--;

            int selectedGrade = optionMap[choice];
            List<Item> selectedItems = items.Where(i => i.Grade == selectedGrade).ToList();
            int maxCount = selectedItems.Count;

            // Get amount to use
            Console.Write($"How many would you like to use? [Max: {maxCount}]: ");
            int count;
            if (!int.TryParse((Console.ReadLine() ?? "").Trim(), out count))
            {
                Console.WriteLine("\nInvalid input. Please only enter a number.");
                return;
            }

            if (count <= 0 || count > maxCount)
            {
                Console.WriteLine($"\nInvalid input. Please only enter a number up to {maxCount}.");
                return;
            }

            // Get character to use on
            Console.Write("Which character would you like to use on? Enter the ID or L to list all characters: ");
            string charId = (Console.ReadLine() ?? "").Trim();

            if (charId.ToLower() == "l")
            {
                ManageCharacters.ListCharacters();
                Console.Write("Which character would you like to use on? Enter the ID: ");
                charId = (Console.ReadLine() ?? "").Trim();
            }

            Similarity charIdTopSuggestion;
            if (!StringUtils.SmartLookUp(charId, db.GetCharacters().Select(c => c.Id).ToList(), out charIdTopSuggestion!))
            {
                Console.WriteLine("\nInvalid input. Unable to find the character.");
                return;
            }

            if (charIdTopSuggestion.QualifiedText.ToLower() != charIdTopSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{charIdTopSuggestion.QualifiedText}'? ({charIdTopSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Character affectedChar = db.GetCharacters().Where(c => c.Id == charIdTopSuggestion.QualifiedText).First();
            foreach (Item i in selectedItems.Take(count))
                i.Use(affectedChar);

            // Delete used items
            db.Items.RemoveRange(selectedItems.Take(count));
            db.SaveChanges();

            Console.WriteLine($"You used {count} {selectedItems[0].Name}s on {affectedChar.Name}.");
            Console.WriteLine($"[x{selectedItems.Count - count} {topSuggestion.QualifiedText} left.]");

            // Award money
            Economy.AwardMoney(5 * count);
            Console.WriteLine($"[+${5 * count}]");
        }
    }

    // Option 8-4: List all items
    private static void ListAllItems()
    {
        using (MushroomContext db = new MushroomContext())
        {
            Profile profile = db.GetProfile(IncludeFlags.Items);

            if (profile.Items.Count == 0)
            {
                Console.WriteLine("\nNo item(s) found. Nothing to list.");
                return;
            }

            foreach (Item i in profile.Items.OrderBy(i => i.Grade))
                EchoItem(i);

            // StdOut metric
            int ExpPotionCount = profile.Items.Where(i => i.Name == "ExpPotion").Count();

            List<string> stdOut = new List<string>();
            if (ExpPotionCount > 0) stdOut.Add($"[x{ExpPotionCount}] ExpPotion(s)");
            if (profile.Items.Count - ExpPotionCount > 0) stdOut.Add($"[x{profile.Items.Count - ExpPotionCount}] HpPotion(s)");
            Console.WriteLine(String.Join(", ", stdOut));
        }
    }

    // Option 8-5: Delete items
    private static void DeleteItems()
    {
        Console.Write(String.Join(
            "\n",
            @"",
            @"(1). Delete item from ID",
            @"(2). Delete item from Name",
            @"(3). Delete item from pattern",
            @"(4). Delete all items",
            @"Please only enter [1, 2, 3, 4] or b to go back: "
        ));

        switch ((Console.ReadLine() ?? "").ToLower())
        {
            case "1":
                DeleteItemFromID();
                break;

            case "2":
                DeleteItemFromName();
                break;

            case "3":
                DeleteItemFromPattern();
                break;

            case "4":
                DeleteAllItems();
                break;

            case "b":
                return;

            default:
                Console.WriteLine("\nInvalid input. Please only enter [1, 2, 3, 4] or b to go back.");
                break;
        }
    }

    // Option 7-5-1: Delete from ID
    private static void DeleteItemFromID()
    {
        // Ask for ID
        Console.Write("Enter Item ID: ");
        string id = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("\nItem ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Item> items = db.GetItems().ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(id, items.Select(i => i.Id.ToString()), out topSuggestion!))
            {
                Console.WriteLine("\nItem not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Item item = items.First(p => p.Id.ToString() == topSuggestion.QualifiedText);

            db.Items.Remove(item);
            db.SaveChanges();

            Console.WriteLine($"Deleted grade {item.Grade} item: {item.Name}");
        }
    }

    // Option 7-5-2: Delete from Name
    private static void DeleteItemFromName()
    {
        // Ask for name
        Console.Write("Enter Item Name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nItem Name cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Item> items = db.Items.ToList();

            Similarity topSuggestion;
            if (!StringUtils.SmartLookUp(name, items.Select(p => p.Name), out topSuggestion!))
            {
                Console.WriteLine("\nItem not found.");
                return;
            }

            if (topSuggestion.QualifiedText.ToLower() != topSuggestion.OriginalText.ToLower())
            {
                Console.Write($"\nDid you mean '{topSuggestion.QualifiedText}'? ({topSuggestion.ScoreToString()}%) [Y/N]: ");
                if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            }

            Item item = items.First(p => p.Name == topSuggestion.QualifiedText);

            db.Items.Remove(item);
            db.SaveChanges();

            Console.WriteLine($"Deleted grade {item.Grade} item: {item.Name}");
        }
    }

    // Option 7-5-3: Delete from pattern
    private static void DeleteItemFromPattern()
    {
        // Ask for pattern
        Console.Write("Enter Item Name or ID: ");
        string pattern = (Console.ReadLine() ?? "").Trim();

        if (String.IsNullOrWhiteSpace(pattern))
        {
            Console.WriteLine("\nItem name or ID cannot be empty.");
            return;
        }

        using (MushroomContext db = new MushroomContext())
        {
            List<Item> items = db.GetItems()
                .Where(p =>
                    p.Id.ToLower().StartsWith(pattern)
                    || p.Name.ToLower().StartsWith(pattern)
                )
                .ToList();

            if (items.Count == 0)
            {
                Console.WriteLine("\nNo item(s) found. Nothing to delete.");
                return;
            }

            // Ask for confirmation
            Console.Write($"Are you sure you want to delete {items.Count} item(s)? [Y/N] or L to list the affected items: ");
            switch ((Console.ReadLine() ?? "").ToLower())
            {
                case "l":
                    foreach (Item item in items)
                        EchoItem(item);

                    // Final confirmation
                    Console.Write("Are you sure you want to delete these items? [Y/N]: ");
                    if ((Console.ReadLine() ?? "").ToLower() != "y") return;
                    break;
                case "y":
                    break;

                default:
                    return;
            }

            // Delete
            db.Items.RemoveRange(items);
            db.SaveChanges();
            Console.WriteLine($"Deleted {items.Count} item(s).");
        }
    }

    // Option 7-5-4: Delete all items
    private static void DeleteAllItems()
    {
        using (MushroomContext db = new MushroomContext())
        {
            Console.Write($"Are you sure you want to delete {db.Items.Count()} item(s)? [Y/N]: ");
            if ((Console.ReadLine() ?? "").ToLower() != "y") return;
            db.Items.RemoveRange(db.Items.ToList());
            db.SaveChanges();

            Console.WriteLine("All items deleted.");
        }
    }
}
