/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

using MushroomPocket.Models;
using MushroomPocket.Utils;

namespace MushroomPocket.Core.DungeonGameLogic;


public static class GameLogic
{
    public static void Start(MushroomContext db, Profile profile, Team team, Dungeon dungeon)
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
            List<string> dmLog = DMAction.DoAction(party, dm).Prepend("[GAME]: DM's turn").ToList();

            // Redraw
            Frame.DrawFrame(party, dm);
            Console.WriteLine(String.Join("\n", dmLog));
            Frame.DrawCountDown(3000);


            // Check for continue
            if (!ContinueGame())
                break;


            // Characters' move
            foreach (PartyMember member in party.PartyMembers.Where(pm => pm.Character.Hp > 0))
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
                            party.PlusAtk.Add(1, 10);
                            party.PlusCritRate.Add(1, 0.1f);
                            logQueue.Add($"[Daisy]: Whole team +10 ATK, +10% CRIT RATE for this turn");
                            break;

                        case "Luigi":
                            member.PlusCritRate.Add(3, 0.5f);
                            logQueue.Add($"[Luigi]: +50% CRIT RATE for next 3 turns");
                            break;

                        case "Mario":
                            member.PlusCritMultiplier.Add(5, 0.1f);
                            logQueue.Add($"[Mario]: +10% CRIT DMG for next 5 turns");
                            break;

                        case "Peach":
                            member.PlusDamageMultiplier.Add(1, 3.5f);
                            member.Stunned.Add(2, 1);
                            attackDM = true;
                            logQueue.Add($"[Peach]: Deals 3.5x DMG for this turn and is stunned for the next 2 turns");
                            break;

                        case "Waluigi":
                            member.PlusDamageMultiplier.Add(1, 0.2f);
                            member.PlusCritRate.Add(2, -0.2f);
                            attackDM = true;
                            logQueue.Add($"[Waluigi]: Deals 2x DMG this turn, -20% CRIT RATE for next 2 turns");
                            break;

                        case "Wario":
                            member.PlusAtk.Add(2, 10);
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

            // Decrement
            party.Decrement();
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

        // Rewards
        int coins = 10 / dungeon.Difficulty;
        int exp = 100;
        int hpPotionCount = 0;
        List<Item> items = new List<Item>();
        List<string> stdOut = new List<string>();

        // Treat as defeated DM
        if (dm.Hp == 0)
        {
            dungeon.Status = "Cleared";

            // Extra rewards
            coins = (int)(dm.MaxHp * (1.5f + new Random().Next(11) / 10f));
            exp = (int)(dm.MaxHp * (2.5f + new Random().Next(11) / 10f));
            for (int i = 0; i < (20 / dungeon.Difficulty); i++)
            {
                if (new Random().Next(100) < 10)
                {
                    items.Add(new HpPotion());
                    hpPotionCount += 1;
                }
                else
                    items.Add(new ExpPotion());
            }

            stdOut.Add($"Congratulations! You have defeated {dungeon.GetDifficulty()} Rank Dungeon {dungeon.Name}!");
        }
        else
        {
            stdOut.Add($"Game over. You have lost the battle.");
        }

        // Add shared stuff
        stdOut.Add($"Damage Taken: {party.TotalDamageTaken}  Damage Dealt: {dm.MaxHp}");
        stdOut.Add($"Coins: ${coins}");
        stdOut.Add($"EXP: {exp}");
        stdOut.Add("");
        stdOut.Add("==== Rewards ====");
        stdOut.Add($"Coins: ${coins}");
        stdOut.Add($"EXP: {exp}");
        stdOut.Add($"HP Potions: {hpPotionCount}x");
        stdOut.Add($"Exp Potions: {items.Count - hpPotionCount}x");
        stdOut.Add("");
        stdOut.Add("==== End ====");

        // StdOut
        Console.WriteLine(PaddingUtils.CenterAlign(String.Join(
            "\n",
            stdOut
        ), stdOut.Max(s => s.Length), "\n"));

        // Update DB
        foreach (Character c in party.GetCharacters())
            c.Exp += exp;
        profile.Wallet += coins;
        profile.BattleLogs.Add(battleLog);
        db.SaveChanges();

        Console.Write("Press any key to continue...");
        Console.ReadKey();

        LoadingHandler endingLoading = Loading.Start();
        Thread.Sleep(3000);
        endingLoading.Dispose();
    }
}
