/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

/* using System.Threading; */
using MushroomPocket.Models;
/* using MushroomPocket.Utils; */

namespace MushroomPocket.Core.DungeonGameLogic;


public static class GameLogic
{
    public static void Start(MushroomContext db, Team team, Dungeon dungeon)
    {
        // Threaded so I can setup some stuff while it shows loading screen in the meantime
        LoadingHandler initialLoadHandler = Loading.Start();
        DungeonMaster dm = new DungeonMaster(dungeon);
        ExplorationParty party = new ExplorationParty(team);

        // Dispose loading screen
        Thread.Sleep(3000);
        initialLoadHandler.Dispose();

        // Helper functions
        bool ContinueGame() => party.IsAllAlive() && dm.Hp != 0;

        // Game loop
        while (ContinueGame())
        {
            // DM moves first
            Frame.DrawFrame(party, dm);

            PartyMember target = party.PickRandomMember();
            float damage = dm.RollDamage();
            target.TakeDamage(damage);

            // Redraw
            Frame.DrawFrame(party, dm);
            Console.WriteLine("[GAME]: DM's turn");
            Console.WriteLine($"[GAME]: DM hits {target.Character.Name} [{target.Character.Id}] with {damage} damage. [{target.Character.Name} has {target.Character.Hp} HP remaining.]");

            Frame.DrawCountDown(3000);


            // Check for continue
            if (!ContinueGame())
                break;


            // Characters' move
            foreach (PartyMember member in party.PartyMembers)
            {
                // Check for stunned
                if (member.IsStunned)
                {
                    Console.WriteLine($"[GAME]: {member.Character.Name} [{member.Character.Id}] is currently stunned. Skipping turn.");
                    Frame.DrawCountDown(3000);
                    continue;
                }

                Character character = member.Character;
                Console.WriteLine($"[GAME]: {character.Name} [{character.Id}]'s turn");

                // Show moveset
                string action;
                while (true)
                {
                    Console.Write(String.Join(
                        "\n",
                        "",
                        $"(1). Attack for {character.Atk} damage [{Math.Round(character.CritRate * 100, 2)}% chance of dealing {character.CritMultiplier}x]",
                        $"(2). Use character skill \"{character.Skill}\" [{character.GetSkillActionText()}]",
                        $"What should {character.Name} do? Enter 1 or 2: "
                    ));

                    string enteredAction = (Console.ReadLine() ?? "").Trim();
                    if (enteredAction != "1" && enteredAction != "2")
                    {
                        Console.WriteLine("\nInvalid input. Please enter 1 or 2.");
                        continue;
                    }

                    action = enteredAction;
                    break;
                }

                // Log
                List<string> logQueue = new List<string>();

                // Handle moveset
                bool attackDM = true;
                if (action == "2")
                {
                    attackDM = false;
                    switch (character.Name)
                    {
                        case "Daisy":
                            party.PlusAtk.Value += 10;
                            party.PlusAtk.Value++;
                            party.PlusCritRate.Value += 0.1f;
                            party.PlusCritRate.RoundsLeft++;
                            logQueue.Add($"[Daisy]: Whole team +10 ATK, +10% CRIT RATE for this turn");
                            break;

                        case "Luigi":
                            member.PlusCritRate.Value += 0.5f;
                            member.PlusCritRate.RoundsLeft += 3;
                            logQueue.Add($"[Luigi]: +50% CRIT RATE for next 3 turns");
                            break;

                        case "Mario":
                            member.PlusCritMultiplier.Value += 0.1f;
                            member.PlusCritMultiplier.RoundsLeft += 5;
                            logQueue.Add($"[Mario]: +10% CRIT DMG for next 5 turns");
                            break;

                        case "Peach":
                            member.PlusDamageMultiplier.Value = 3.5f;
                            member.PlusDamageMultiplier.RoundsLeft = 1;
                            member.Stunned.Value = true;
                            member.Stunned.RoundsLeft = 2;
                            logQueue.Add($"[Peach]: Deals 3.5x DMG for this turn and is stunned for the next 2 turns");
                            break;

                        case "Waluigi":
                            member.PlusDamageMultiplier.Value += 2f;
                            member.PlusDamageMultiplier.RoundsLeft += 1;
                            member.PlusCritRate.Value -= 0.2f;
                            member.PlusCritRate.RoundsLeft += 2;
                            logQueue.Add($"[Waluigi]: +2 DMG for next 1 turn, -20% CRIT RATE for next 2 turns");
                            break;

                        case "Wario":
                            party.PlusAtk.Value += 10;
                            party.PlusAtk.RoundsLeft += 2;
                            logQueue.Add($"[Wario]: Whole team +10 ATK, +2 turns");
                            break;

                        default:
                            throw new NotImplementedException($"{character.Name} doesn't have a skill yet.");
                    }
                }

                // Damage/Use skill
                if (attackDM)
                {
                    float charDamage = party.RollDamage(member);
                    dm.TakeDamage(charDamage);
                    logQueue.Add($"[{character.Name}]: Inflicts DM with {charDamage} damage. [DM has {dm.Hp} HP remaining.]");
                }

                // Redraw
                Frame.DrawFrame(party, dm);

                // StdOut action
                Console.WriteLine(String.Join("\n", logQueue));

                // Wait
                Frame.DrawCountDown(3000);

                // Check for continue
                if (!ContinueGame())
                    break;
            }
        }

        // End of dungeon
        List<Character> dead = party.GetDeadCharacters();
        BattleLog battleLog = new BattleLog(
            party.PartyMembers.Count,
            dead.Count,
            dm.Hp == 0,
            dungeon.GetDifficulty(),
            (int)(dm.MaxHp - dm.Hp),
            (int)(party.TotalDamageTaken)
        );

        Profile profile = db.GetProfile(IncludeFlags.BattleLogs);

        // Treat as defeated DM
        if (dm.Hp == 0)
        {
            Console.Clear();

            // Generate rewards
            int coins = (int)(dm.MaxHp * 1.5f);
            int exp = (int)(dm.MaxHp * 2.5f);

            foreach (Character c in party.GetCharacters())
            {
                c.Exp += exp;
            }

            profile.Wallet += coins;

            string congratsText = $"Congratulations! You have successfully defeated {dungeon.GetDifficulty()} Rank Dungeon {dungeon.Name}!";
            Console.WriteLine(Frame.CenterAlign(String.Join(
                "\n",
                "",
                congratsText,
                "",
                $"Damage Taken: {party.TotalDamageTaken}  Damage Dealt: {dm.MaxHp}",
                dead.Count > 0 ? $"Dead: {String.Join(", ", dead.Select(x => x.Name))}" : "",
                "",
                "==== Rewards ====",
                $"Coins: ${coins}",
                $"EXP: {exp}",
                ""
            ), congratsText.Length));
            return;
        }

        profile.BattleLogs.Add(battleLog);
        db.SaveChanges();

        // Treat as all dead
        Console.Clear();
        Console.WriteLine("[GAME]: All characters are dead. Game over.");
        Console.WriteLine("\nA mysterious force sighs before expelling you from the dungeon\n");
    }
}
