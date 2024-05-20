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
        int tagCount = (int)Math.Ceiling((barWidth - 4) * (hp / maxHp));
        int spaceCount = (int)Math.Ceiling((float)barWidth - 4 - tagCount);

        return String.Join(
            "\n",
            CenterAlign($"Hp: {hp}/{maxHp}", barWidth),
            $"| {String.Join("", Enumerable.Repeat("#", tagCount))}{String.Join("", Enumerable.Repeat(" ", spaceCount))} |"
        );
    }


    public static string CenterAlign(string s, int width)
    {
        List<string> formatted = new List<string>();

        foreach (string line in s.Split("\n"))
        {
            if (line.Length == 0)
            {
                formatted.Add(line);
                continue;
            }

            int leading = (int)Math.Floor((decimal)(width - line.Length) / 2);
            leading = Math.Clamp(leading, 0, s.Length);

            formatted.Add(
                String.Join("", Enumerable.Repeat(" ", leading))
                + line
                + String.Join("", Enumerable.Repeat(" ", leading))
            );
        }

        return String.Join("\n", formatted);
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


    public static void DrawFrame(Team team, DungeonMaster dm)
        => DrawFrame(team.Characters, dm);
    public static void DrawFrame(ExplorationParty explorationParty, DungeonMaster dm)
        => DrawFrame(explorationParty.GetCharacters(), dm);
    public static void DrawFrame(IEnumerable<Character> characters, DungeonMaster dm)
    {
        Console.Clear();
        Dimension canvas = ResolveDimension();

        string frame = "";

        // Build dungeon DungeonMaster
        frame += CenterAlign(String.Join(
            "\n",
            ASCIIArt.DungeonMaster,
            GenerateHpIndicator(ASCIIArt.DungeonMasterDimensions, dm.Hp, dm.MaxHp),
            dm.Name,
            $"Atk: {dm.Atk}",
            "",
            ""
        ), canvas.X);

        // Build team
        int teamSpacing = 4;
        List<List<string>> teamMembers = new List<List<string>>();
        Dimension teamASCIIDimension = new Dimension(ASCIIArt.CharacterDimensions.X * 3, ASCIIArt.CharacterDimensions.Y);

        foreach (Character character in characters)
        {
            string ascii = String.Join(
                "\n",
                ASCIIArt.Character,
                GenerateHpIndicator(teamASCIIDimension, character.Hp, character.MaxHp),
                character.Name,
                $"Atk: {character.Atk}"
            );
            teamMembers.Add(CenterAlign(ascii, teamASCIIDimension.X).Split("\n").ToList());
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
