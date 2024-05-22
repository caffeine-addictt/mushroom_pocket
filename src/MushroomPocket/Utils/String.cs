/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Utils;


// Classes
public class Similarity
{
    /// <summary>
    /// The score of the comparison.
    /// A value of 1 means a 100% confidence.
    /// </summary>
    public double Score;

    /// <summary>
    /// The text being compared. The "incorrect" one.
    /// </summary>
    public string OriginalText;

    /// <summary>
    /// The text being compared against. The "correct" one.
    /// </summary>
    public string QualifiedText;


    public Similarity(string qualifiedText, string originalText, double score)
    {
        Score = score;
        OriginalText = originalText;
        QualifiedText = qualifiedText;
    }

    public string ScoreToString() => Math.Round(Score * 100, 2).ToString();
}


// Utils
public static class StringUtils
{
    /// <summary>
    /// Clean a string
    /// </summary>
    public static string Clean(string input, bool lowerCase = false)
        => lowerCase
            ? input.Trim().ToLower()
            : input.Trim();
    public static string Clean(char input, bool lowerCase = false)
        => lowerCase
            ? input.ToString().Trim().ToLower()
            : input.ToString().Trim();


    /// <summary>
    /// Generate a tiny ID with hit detection
    /// Is not thread safe
    ///
    /// <paramref name="fetchItemCount"/> is used to fetch the number of items in the table
    /// <paramref name="hitDetection"/> is used to check if the id is already in the table
    /// </summary>
    public static string TinyId(IEnumerable<string> existing)
    {
        int length = 6;
        string id;
        void newId()
        {
            id = Guid.NewGuid().ToString().Substring(0, length);
        }

        newId();
        while (existing.Contains(id))
            newId();

        return id;
    }


    /// <summary>
    /// Tries to find the closes match from an input string and a list of possible matches.
    ///
    /// <paramref name="results"/> length is 0 if no match was found, meaning it returns false.
    /// </summary>
    public static bool SmartLookUp(string input, IEnumerable<string> possibleMatches, out List<Similarity> results)
    {
        // Only include matches that are at least 30% of the length of the input
        float acceptThreshold = 0.3f;

        // Assigning weights to each rule, should add up to 1.0
        float lengthWeight = 0.15f;
        float startWeight = 0.25f;
        float subStrWeight = 0.25f;
        float charWeight = 0.35f;

        results = new List<Similarity>();
        foreach (string possibleMatch in possibleMatches)
        {
            // input is perfect match
            if (Clean(possibleMatch, true) == Clean(input, true))
            {
                results = new List<Similarity> { new Similarity(possibleMatch, input, 1.0f) };
                return true;
            }

            // Calculate score
            double score = 0;
            bool hasSub = false;
            bool hasStart = false;

            string cleanedInput = Clean(input);
            string cleanedPossibleMatch = Clean(possibleMatch);

            // Length
            // Multiply weight by % difference of length relative to possible match
            score += (Math.Abs(cleanedPossibleMatch.Length - cleanedInput.Length) / cleanedPossibleMatch.Length) * lengthWeight;

            for (int i = 0; i < cleanedInput.Length; i++)
            {
                // Do back tracking to get the largest match
                // aaaaaaaa
                // aaaaaaa
                // aaaaaa
                // ...
                string sub = cleanedInput.Substring(cleanedInput.Length - i - 1);

                // Starts with
                // Multiply weight with % length of input if it is at the start of the possible match
                if (!hasStart)
                {
                    // Give more for case sensitive match
                    if (cleanedPossibleMatch.StartsWith(sub))
                    {
                        score += startWeight * ((cleanedInput.Length - i) / cleanedInput.Length);
                        hasStart = true;
                    }
                    else if (Clean(cleanedPossibleMatch, true).StartsWith(Clean(sub, true)))
                    {
                        // Penalize 5% for not case sensitive
                        score += (startWeight - 0.05f) * ((cleanedInput.Length - i) / cleanedInput.Length);
                        hasStart = true;
                    }
                }

                // Sub string
                // For input of fff, if possible match is ddddddfffddd, this will match
                // Multiply weight with % length of input if it is a sub string of the possible match
                if (!hasSub)
                {
                    // Give more for case sensitive match
                    if (cleanedPossibleMatch.Contains(sub))
                    {
                        score += subStrWeight * ((cleanedInput.Length - i) / cleanedInput.Length);
                        hasSub = true;
                    }
                    else if (Clean(cleanedPossibleMatch, true).Contains(Clean(sub, true)))
                    {
                        // Penalize 5% for not case sensitive
                        score += (subStrWeight - 0.05f) * ((cleanedInput.Length - i) / cleanedInput.Length);
                        hasSub = true;
                    }
                }

                // Has character
                // 80% of weight/length of input for non case sensitive match
                // 20% of weight/length of input for case sensitive match
                if (cleanedPossibleMatch.Contains(cleanedInput[i]))
                {
                    score += charWeight / cleanedInput.Length;
                }
                else if (Clean(cleanedPossibleMatch, true).Contains(Clean(cleanedInput[i], true)))
                {
                    score += (charWeight * 0.8f) / cleanedInput.Length;
                }
            }

            // Decide adding to Similarity
            if (score >= acceptThreshold) results.Add(new Similarity(possibleMatch, input, score));
        }


        // Return only if there are matches
        if (results.Count > 0)
        {
            // Sort by score descending
            results.Sort((a, b) => b.Score.CompareTo(a.Score));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Does the same but only cares if there is a possible match.
    /// <see cref="SmartLookUp"/>
    /// </summary>
    public static bool SmartLookUp(string input, IEnumerable<string> possibleMatches)
        => SmartLookUp(input, possibleMatches, out List<Similarity> _);

    /// <summary>
    /// Does the same with the closest match.
    /// <see cref="SmartLookUp"/>
    /// </summary>
    public static bool SmartLookUp(string input, IEnumerable<string> possibleMatches, out Similarity? closestMatch)
    {
        List<Similarity> results;
        bool hasMatch = SmartLookUp(input, possibleMatches, out results);

        if (!hasMatch)
        {
            closestMatch = null;
            return false;
        }

        closestMatch = results[0];
        return true;
    }
}
