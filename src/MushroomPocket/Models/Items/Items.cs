/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using Microsoft.EntityFrameworkCore;
using MushroomPocket.Utils;

namespace MushroomPocket.Models;


[PrimaryKey("Id")]
public class Item
{
    public string Id { get; set; }

    public string Name { get; set; } = null!;
    public int Grade { get; set; } // Lower the grade, higher the priority

    public virtual Profile Profile { get; set; } = null!;


    public Item()
    {
        Id = GenerateId();
        Grade = new Random().Next(1, 4);
    }


    /// <summary>
    /// Get description
    /// </summary>
    public string GetDescription() => GetDescription(this);
    public static string GetDescription(Item item) => GetDescription(item.Name);
    public static string GetDescription(string itemName)
    {
        switch (itemName)
        {
            case "HpPotion":
                return "Poisonous mushrooms, Cyanide and some weird cloud. Forget healing - they lived?!";

            case "ExpPotion":
                return "Induces strangely vivid dreams. They claim not to remember anything, but they seem.. stronger...";

            default:
                throw new Exception("No description for item: " + itemName);
        }
    }


    /// <summary>
    /// Get price
    /// </summary>
    public int GetPrice() => GetPrice(this);
    public static int GetPrice(Item item) => GetPrice(item.Name);
    public static int GetPrice(string itemName)
    {
        switch (itemName)
        {
            case "HpPotion":
                return 10;

            case "ExpPotion":
                return 20;

            default:
                throw new Exception("No price for item: " + itemName);
        }
    }


    /// <summary>
    /// Get success message
    /// StdOut when item is successfully used
    /// </summary>
    public string GetSuccessMessage() => GetSuccessMessage(this);
    public static string GetSuccessMessage(Item item)
    {
        switch (item.Name)
        {
            case "HpPotion":
                return $"Grade {item.Grade} Hp Potion healed {item.Calculate()}hp!";

            case "ExpPotion":
                return $"Increased experience by {item.Calculate()}!";

            default:
                throw new Exception("No success message for item: " + item.Name);
        }
    }


    /// <summary>
    /// Get effect description
    /// </summary>
    public string GetEffectDescription() => Item.GetEffectDescription(this);
    public static string GetEffectDescription(Item item) => GetEffectDescription(item.Name, item.Grade);
    public static string GetEffectDescription(string name, int grade)
    {
        switch (name)
        {
            case "HpPotion":
                return $"Recovers {Calculate(name, grade)} of character hp!";

            case "ExpPotion":
                return $"Increases character exp by {Calculate(name, grade)}!";

            default:
                throw new Exception("No effect description for item: " + name);
        }
    }


    /// <summary>
    /// Use the item
    /// </summary>
    public void Use(Character c)
    {
        switch (Name)
        {
            case "HpPotion":
                c.Hp += Calculate();
                break;

            case "ExpPotion":
                c.Exp += (int)Calculate();
                break;
        }
    }


    /// <summary>
    /// Calculate effectiveness
    /// </summary>
    public float Calculate() => Calculate(Name, Grade);
    public static float Calculate(string name, int grade)
    {
        switch (name)
        {
            case "HpPotion":
                return (float)Math.Floor((decimal)50 / grade); // Max 50 HP

            case "ExpPotion":
                return (float)Math.Floor((decimal)100 / grade); // Max 100 EXP

            default:
                throw new Exception("No calculation for item: " + name);
        }
    }


    /// <summary>
    /// Generate id
    /// </summary>
    private static string GenerateId(MushroomContext db)
    {
        List<string> ids = db.Items.Select((Item i) => i.Id).ToList();
        return StringUtils.TinyId(ids);
    }
    private static string GenerateId()
    {
        using (MushroomContext db = new MushroomContext())
            return GenerateId(db);
    }
}
