/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

/* using System.Threading; */
using MushroomPocket.Models;
using MushroomPocket.Utils;

namespace MushroomPocket.Core.DungeonGameLogic;


public static class Frame
{
    public static Dimension ResolveDimension()
        => new Dimension(ASCIIArt.DungeonMasterDimensions.X * 2, ASCIIArt.DungeonMasterDimensions.Y * 2 + 4);


    public static string GenerateHpIndicator(Dimension xy, float hp, float maxHp)
    {
        string hpText = $"Hp: {hp}/{maxHp}";
        int barWidth = Math.Max(Math.Max((int)Math.Floor((double)xy.X / 1.5), 4), hpText.Length);
        int tagCount = (int)Math.Max(Math.Ceiling((barWidth - 4) * (hp / maxHp)), 0);
        int spaceCount = (int)Math.Max(Math.Ceiling((float)barWidth - 4 - tagCount), 0);

        return String.Join(
            "\n",
            PaddingUtils.CenterAlign(hpText, barWidth, "\n"),
            $"| {String.Join("", Enumerable.Repeat("#", tagCount))}{String.Join("", Enumerable.Repeat(" ", spaceCount))} |"
        );
    }


    public static void DrawCountDown(int ms, int intervalMs = 1000)
    {
        DateTime start = DateTime.UtcNow;
        DateTime end = start.AddMilliseconds(ms);
        TimeSpan TotalMilliseconds = end - start;

        Console.Write("\nContinuing in: ");
        while (true)
        {
            TimeSpan elapsed = DateTime.UtcNow - start;
            if (TotalMilliseconds.Seconds - elapsed.Seconds <= 0) break;

            Console.Write($"{TotalMilliseconds.Seconds - elapsed.Seconds}s.. ");
            Thread.Sleep(intervalMs);
        }
        Console.WriteLine("\n");
    }


    public static void DrawFrame(ExplorationParty party, DungeonMaster dm)
    {
        Console.Clear();
        Dimension canvas = ResolveDimension();

        string frame = "";

        // Build dungeon DungeonMaster
        frame += PaddingUtils.CenterAlign(String.Join(
            "\n",
            ASCIIArt.DungeonMaster,
            GenerateHpIndicator(ASCIIArt.DungeonMasterDimensions, dm.Hp, dm.MaxHp),
            dm.Name,
            $"Atk: {Math.Floor((decimal)dm.Atk)}{(dm.IsAtk ? $" + {Math.Floor((decimal)dm.PlusAtk.Value)}" : "")}",
            $"Crit Rate: 50%{(dm.IsCritRate ? $" + {(int)Math.Round(dm.PlusCritRate.Value * 10, 2)}%" : "")}",
            $"Crit Dmg: 1.5x{(dm.IsDamageMultiplier ? $" + {(int)Math.Floor(dm.PlusCritMultiplier.Value)}x" : "")}",
            $"Dmg Multiplier: 1x{(dm.IsDamageMultiplier ? $" + {dm.PlusDamageMultiplier.Value}x" : "")}",
            "",
            ""
        ), canvas.X, "\n");

        // Build team
        int teamSpacing = 4;
        List<List<string>> teamMembers = new List<List<string>>();
        Dimension teamASCIIDimension = new Dimension(ASCIIArt.CharacterDimensions.X * 3, ASCIIArt.CharacterDimensions.Y);

        foreach (PartyMember member in party.PartyMembers)
        {
            Character character = member.Character;
            List<string> ascii = new List<string>() {
                ASCIIArt.Character,
                GenerateHpIndicator(teamASCIIDimension, character.Hp, character.MaxHp),
                character.Name,
                $"Atk: {Math.Floor((decimal)character.Atk)}{(member.IsAtk ? $" + {Math.Floor(member.PlusAtk.Value)}" : "")}",
                $"Crit Rate: {Math.Round((decimal)character.CritRate * 10, 2)}%{(member.IsCritRate ? $" + {(int)Math.Round(member.PlusCritRate.Value * 10, 2)}%" : "")}",
                $"Crit Dmg: {Math.Floor(character.CritMultiplier + member.PlusCritMultiplier.Value)}x{(member.IsDamageMultiplier ? $" + {member.PlusDamageMultiplier.Value}x" : "")}",
                $"Dmg Multiplier: 1x{(member.IsDamageMultiplier ? $" + {member.PlusDamageMultiplier.Value}x" : "")}",
                member.IsStunned ? "Stunned" : ""
            };

            teamMembers.Add(PaddingUtils.CenterAlign(String.Join("\n", ascii), teamASCIIDimension.X, "\n").Split("\n").ToList());
        }

        // Join
        List<string> formattedLines = new List<string>();
        for (int i = 0; i < teamMembers[0].Count; i++)
        {
            List<string> lineFragment = teamMembers.Select(x => x[i]).ToList();
            formattedLines.Add(String.Join(String.Join("", Enumerable.Repeat(" ", teamSpacing)), lineFragment));
        }

        frame += String.Join("\n", formattedLines);

        // Display party-wide
        List<string> partyBuffs = new List<string>();
        if (party.IsAtk) partyBuffs.Add($"+{party.PlusAtk.Value} ATK");
        if (party.IsCritRate) partyBuffs.Add($"x{party.PlusCritRate.Value} CRIT");
        if (party.IsCritMultiplier) partyBuffs.Add($"x{party.PlusCritMultiplier.Value} CRIT DMG");
        if (party.IsDamageMultiplier) partyBuffs.Add($"x{party.PlusDamageMultiplier.Value} DMG");
        if (partyBuffs.Count > 0) frame += "\n\n" + String.Join("\n", partyBuffs);

        // Pad Y-Axis
        Console.WriteLine($"\n\n{frame}\n\n");
    }
}
