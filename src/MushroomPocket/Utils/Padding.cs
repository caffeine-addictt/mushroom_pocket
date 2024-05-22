/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Utils;


// Classes
public struct Padded
{
    /// <summary>
    /// The number of leading.
    ///
    /// Always between 0 and <see cref="Trailing"/> - 1
    /// </summary>
    public int Leading { get; set; }

    /// <summary>
    /// The number of trailing.
    ///
    /// Always between 0 and <see cref="Leading"/> + 1
    /// </summary>
    public int Trailing { get; set; }

    /// <summary>
    /// The number between <see cref="Leading"/> and <see cref="Trailing"/>
    /// </summary>
    public int Middle { get; set; }


    /// <summary>
    /// Total length
    /// </summary>
    public int Length => Leading + Middle + Trailing;


    public Padded() { }
    public Padded(int leading, int middle, int trailing)
    {
        Leading = leading;
        Middle = middle;
        Trailing = trailing;
    }
}


// Utils
public static class PaddingUtils
{
    /// <summary>
    /// Base implementation of Pad
    /// </summary>
    public static Padded Pad(int s, int targetLength)
    {
        // Account for s is already too long
        if (s >= targetLength) return new Padded(0, s, 0);

        int totalPadding = targetLength - s;
        int leading = totalPadding / 2;
        int trailing = totalPadding - leading;
        return new Padded(leading, s, trailing);
    }

    /// <summary>
    /// String implementation of pad
    /// </summary>
    public static string Pad(string s, int targetLength, char c)
    {
        Padded padded = Pad(s.Length, targetLength);
        return new string(c, padded.Leading) + s + new string(c, padded.Trailing);
    }


    /// <summary>
    /// Base implementation of CenterAlign
    /// </summary>
    public static string CenterAlign(string s, int targetLength, char c)
        => Pad(s, targetLength, c);

    /// <summary>
    /// Invokes CenterAlign with ' '
    ///
    /// <see cref="CenterAlign(string, int, char)"/>
    /// </summary>
    public static string CenterAlign(string s, int targetLength)
        => CenterAlign(s, targetLength, ' ');

    /// <summary>
    /// Splits the string by <paramref name="splitBy"/> and treats each as its own line,
    /// then calls for <see cref="CenterAlign(string, int)"/>
    ///
    /// Passing either <paramref name="targetLength" /> or the length of the longest line as the second argument,
    /// whichever is larger will be used.
    ///
    /// Also does not pad empty lines.
    ///
    /// <seealso cref="CenterAlign(string, int)"/>
    /// </summary>
    public static string CenterAlign(string s, int targetLength, string splitBy)
    {
        string[] lines = s.Split(splitBy);
        int targetLineLength = Math.Max(lines.Max(x => x.Length), targetLength);

        List<string> formatted = new List<string>();
        foreach (string line in lines)
            formatted.Add(line.Length == 0
                ? line
                : CenterAlign(line, targetLineLength)
            );

        return String.Join(splitBy, formatted);
    }
}
