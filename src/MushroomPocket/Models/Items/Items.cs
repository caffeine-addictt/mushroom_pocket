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

    public string Name { get; set; } = "";
    public int Grade { get; set; } // Lower the grade, higher the priority

    public static int Price { get; set; }
    public const string Description { get; set; } = "";

    public virtual Profile Profile { get; set; } = null!;

    public Item()
    {
        Id = GenerateId();
        Grade = new Random().Next(0, 3);
    }

    public Item(string name, string description, int price) : this()
    {
        Name = name;
        Price = price;
        Description = description;
    }

    /// <summary>
    /// Success echo
    /// </summary>
    public virtual string SuccessEcho()
        => "";

    /// <summary>
    /// Use the item
    /// </summary>
    public virtual void Use(Character c) { }


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
