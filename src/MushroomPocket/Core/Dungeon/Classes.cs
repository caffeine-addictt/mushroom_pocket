/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;

namespace MushroomPocket.Core;


public class Effect<T>
{
    public int RoundsLeft { get; set; }
    public T Value { get; set; }
    public T DefaultValue { get; private set; }


    public Effect(int rounds, T defaultValue)
    {
        RoundsLeft = rounds;
        Value = defaultValue;
        DefaultValue = defaultValue;
    }


    /// <summary>Decrements rounds left</summary>
    public void Decrement()
        => RoundsLeft = Math.Max(RoundsLeft - 1, 0);


    /// <summary>
    /// Syntatic sugar for whether to apply an effect
    /// </summary>
    public bool ShouldApply() => ShouldApply(this);
    public static bool ShouldApply(Effect<T> e) => e.RoundsLeft > 0;
}


public class IHasEffect
{
    /// <summary>Value representing how much to add to atk</summary>
    public Effect<float> PlusAtk { get; set; } = new Effect<float>(0, 0);
    /// <summary>Value representing how much to add to base crit rate %</summary>
    public Effect<float> PlusCritRate { get; set; } = new Effect<float>(0, 0);
    /// <summary>Value representing how much to add to base crit multiplier %</summary>
    public Effect<float> PlusCritMultiplier { get; set; } = new Effect<float>(0, 0);
    /// <summary>Value representing how much to total damage multiplier %</summary>
    public Effect<float> PlusDamageMultiplier { get; set; } = new Effect<float>(0, 1);


    public bool IsAtk => PlusAtk.Value != 0;
    public bool IsCritRate => PlusCritRate.Value != 0;
    public bool IsCritMultiplier => PlusCritMultiplier.Value != 0;
    public bool IsDamageMultiplier => PlusDamageMultiplier.Value != 1;


    /// <summary>
    /// Calculates damage to inflict
    ///
    /// <paramref name="p"/> is the member initiating the action
    /// </summary>
    public float RollDamage(PartyMember p) => RollDamage(this, p.Character);
    public float RollDamage(Character c) => RollDamage(this, c);
    public float RollDamage(DungeonMaster m) => RollDamage(this, m);

    public static float RollDamage(IHasEffect e, Character c) => RollDamage(e, c.Atk, c.CritRate, c.CritMultiplier);
    public static float RollDamage(IHasEffect e, DungeonMaster m) => RollDamage(e, m.Atk, 0.5f, 1.5f);
    public static float RollDamage(IHasEffect e, int atk, float critRate, float critMultiplier)
    {
        float finalDamage = atk;

        // Handle Atk
        if (e.IsAtk)
        {
            finalDamage += e.PlusAtk.Value;
            e.PlusAtk.Decrement();
        }

        // Handle Crit
        float newCriteRate = critRate * 10;
        if (e.IsCritRate)
        {
            newCriteRate += e.PlusCritRate.Value;
            e.PlusCritRate.Decrement();
        }

        // Do crit?
        if (newCriteRate >= new Random().Next(11))
        {
            float newCriteMultiplier = critMultiplier;
            // Handle Crit Multiplier
            if (e.IsCritMultiplier)
            {
                critMultiplier += e.PlusCritMultiplier.Value;
                e.PlusCritMultiplier.Decrement();
            }
            finalDamage *= newCriteMultiplier;
        }

        // Handle Damage Multiplier
        if (e.IsDamageMultiplier)
        {
            finalDamage *= e.PlusDamageMultiplier.Value;
            e.PlusDamageMultiplier.Decrement();
        }

        return finalDamage;
    }
}


public class DungeonMaster : IHasEffect
{
    public readonly string Name = "Dungeon Master";
    public readonly string Description = "The master of the dungeon. He manages the dungeon and keeps it safe. Good luck!";
    public int Atk { get; set; }
    public float Hp { get; set; }
    public float MaxHp { get; private set; }

    public DungeonMaster(Dungeon dungeon)
    {
        Hp = 100 / dungeon.Difficulty;
        Atk = 20 / dungeon.Difficulty;
        MaxHp = Hp;
    }

    public float RollDamage() => RollDamage(this);

    public void TakeDamage(float damage) => TakeDamage((int)damage);
    public void TakeDamage(int damage)
        => Hp = Math.Max(0, Hp - damage);
}


public class PartyMember : IHasEffect
{
    /// <summary>Value representing if is stunned</summary>
    public Effect<bool> Stunned { get; set; } = new Effect<bool>(0, false);
    public bool IsStunned => Stunned.Value;

    public Character Character { get; set; }


    public PartyMember(Character c)
    {
        this.Character = c;
    }


    public float RollDamage() => RollDamage(this);

    public void TakeDamage(float damage) => TakeDamage((int)damage);
    public void TakeDamage(int damage)
        => Character.Hp = Math.Max(0, Character.Hp - damage);
}


public class ExplorationParty : IHasEffect
{
    public float TotalDamageTaken { get; set; } = 0;
    public List<PartyMember> PartyMembers { get; set; }


    public ExplorationParty(Team team)
    {
        // Create party members
        List<PartyMember> members = new List<PartyMember>();
        foreach (Character c in team.Characters)
            members.Add(new PartyMember(c));
        PartyMembers = members;
    }


    public List<Character> GetCharacters() => PartyMembers.Select(pm => pm.Character).ToList();
    public List<Character> GetAliveCharacters() => PartyMembers.Where(pm => pm.Character.Hp > 0).Select(pm => pm.Character).ToList();
    public bool IsAllAlive() => PartyMembers.All(pm => pm.Character.Hp > 0);
    public PartyMember PickRandomMember() => PartyMembers[new Random().Next(PartyMembers.Count)];

    public void Damage(DungeonMaster dm, float damage)
    {
        dm.Hp -= (int)damage;
        TotalDamageTaken += damage;
    }
}