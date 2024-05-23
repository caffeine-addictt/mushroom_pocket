/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Core.DungeonGameLogic;


public class BossAction
{
    public int Weight { get; set; }
    public Func<ExplorationParty, DungeonMaster, List<string>> Use { get; set; }

    public BossAction(int weight, Func<ExplorationParty, DungeonMaster, List<string>> use)
    {
        Weight = weight;
        Use = use;
    }
}


// Weighted random to reduce infinite stun occurence
public static class DMAction
{
    public static List<BossAction> Actions { get; private set; } = new List<BossAction>() {
        // Damage target
        new BossAction(
            7,
            (ExplorationParty party, DungeonMaster dm) => {
                PartyMember target = party.PickRandomMember();
                float damage = dm.RollDamage();
                party.TakeDamage(target, damage);

                return new List<string>() { $"[GAME]: DM hits {target.Character.Name} [{target.Character.Id}] with {damage} damage. [{target.Character.Name} has {target.Character.Hp} HP remaining.]" };
            }
        ),

        // Stun target
        new BossAction(
            3,
            (ExplorationParty party, DungeonMaster dm) => {
                PartyMember target = party.PickRandomMember();
                target.Stunned.Add(1, 1);

                return new List<string>() { $"[GAME]: DM stunned {target.Character.Name} [{target.Character.Id}] for this turn" };
            }
        ),

        // Skill 1: Debuff party
        new BossAction(
            2,
            (ExplorationParty party, DungeonMaster dm) => {
                party.PlusDamageMultiplier.Add(1, -0.1f);
                return new List<string>() {
                    "[GAME]: DM uses its skill: Debuff",
                    "[GAME]: Party -10% DMG for this turn"
                };
            }
        ),

        // Skill 2: Knock-out
        new BossAction(
            2,
            (ExplorationParty party, DungeonMaster dm) => {
                PartyMember target = party.PickRandomMember();
                target.Stunned.Add(2, 1);
                return new List<string>() {
                    "[GAME]: DM uses its skill: Knock-out",
                    $"[GAME]: {target.Character.Name} [{target.Character.Id}] is knocked out for 2 turns"
                };
            }
        ),

        // Skill 3: Buff
        new BossAction(
            1,
            (ExplorationParty party, DungeonMaster dm) => {
                dm.PlusCritRate.Add(2, 0.2f);
                dm.PlusCritMultiplier.Add(2, 0.5f);
                return new List<string>() {
                    "[GAME]: DM uses its skill: Buff",
                    "[GAME]: DM +20% CRIT RATE and +50% CRIT DMG for the next 2 turns"
                };
            }
        ),

        // Miss
        new BossAction(
            1,
            (ExplorationParty party, DungeonMaster dm) => {
                return new List<string>() { "[GAME]: DM missed" };
            }
        )
    };


    /// <summary>
    /// Do Action
    /// </summary>
    public static List<string> DoAction(ExplorationParty party, DungeonMaster dm)
    {
        int totalWeight = Actions.Sum(a => a.Weight);
        int randomNum = new Random().Next(0, totalWeight);

        int cumulativeWeight = 0;
        foreach (var action in Actions)
        {
            cumulativeWeight += action.Weight;
            if (randomNum < cumulativeWeight)
            {
                return action.Use(party, dm);
            }
        }
        return new List<string>() { "[GAME]: DM is dazed" };
    }
}
