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


public static class Frame
{
    public static Dimension ResolveDimension()
        => new Dimension(ASCIIArt.DungeonMasterDimensions.X * 2, ASCIIArt.DungeonMasterDimensions.Y * 2 + 4);


    public static string GenerateHpIndicator(Dimension xy, float hp, float maxHp)
    {
        int barWidth = Math.Max((int)Math.Floor((double)xy.X / 1.5), 4);

        return String.Join(
            "\n",
            CenterAlign($"Hp: {hp}/{maxHp}", barWidth),
            $"| {String.Join("", Enumerable.Repeat("#", barWidth - 4))} |"
        );
    }


    public static string CenterAlign(string s, int width)
    {
        List<string> formatted = new List<string>();

        foreach (string line in s.Split("\n"))
        {
            int leading = (int)Math.Floor((decimal)(width - line.Length) / 2);
            leading = Math.Clamp(leading, 0, s.Length);

            formatted.Add(
                String.Join("", Enumerable.Repeat(" ", leading))
                + line
                + String.Join("", Enumerable.Repeat(" ", width - line.Length - leading))
            );
        }

        return String.Join("\n", formatted);
    }


    public static void DrawFrame(Team team, DungeonMaster dm)
    {
        Console.Clear();
        Dimension canvas = ResolveDimension();

        string frame = "";

        // Build dungeon DungeonMaster
        frame += CenterAlign(ASCIIArt.DungeonMaster, canvas.X) + "\n";
        frame += CenterAlign(GenerateHpIndicator(ASCIIArt.DungeonMasterDimensions, dm.Hp, dm.MaxHp), canvas.X) + "\n\n\n";

        // Build team
        int teamSpacing = 2;
        List<List<string>> teamMembers = new List<List<string>>();
        Dimension teamASCIIDimension = new Dimension(ASCIIArt.CharacterDimensions.X * 2, ASCIIArt.CharacterDimensions.Y);

        foreach (Character character in team.Characters)
        {
            string ascii = String.Join(
                "\n",
                CenterAlign(ASCIIArt.Character, teamASCIIDimension.X),
                CenterAlign(GenerateHpIndicator(teamASCIIDimension, character.Hp, character.MaxHp), teamASCIIDimension.X)
            );
            teamMembers.Add(ascii.Split("\n").ToList());
        }

        // Join
        List<string> formattedLines = new List<string>();
        for (int i = 0; i < teamMembers[0].Count; i++)
        {
            List<string> lineFragment = teamMembers.Select(x => x[i]).ToList();
            formattedLines.Add(String.Join(String.Join("", Enumerable.Repeat(" ", teamSpacing)), lineFragment));
        }

        frame += String.Join("\n", formattedLines);

        // Pad Y-Axis
        Console.WriteLine($"\n\n{frame}\n\n");
    }
}
