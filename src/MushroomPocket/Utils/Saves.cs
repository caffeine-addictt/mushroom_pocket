/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Utils;

public static class SaveUtils
{
    /// <summary>
    /// Save directory path
    /// </summary>
    public static readonly string SaveDirPath = GetSaveDirPath();

    /// <summary>
    /// Save file pattern
    /// </summary>
    public static readonly string SaveFilePattern = "*.db";

    /// <summary>
    /// New save file pattern
    /// </summary>
    private static readonly string NewSaveFilePattern = @"{0}.db";

    /// <summary>
    /// Current db name
    /// </summary>
    public static readonly string CurrentDbName = Path.Combine(GetDirPath(), "mushroom.db");

    /// <summary>
    /// Get directory path
    /// </summary>
    private static string GetDirPath()
    {
        string workingDir = Environment.CurrentDirectory;

        // See if being executed from project root (development)
        if (Path.Exists(Path.Combine(workingDir, "mushroom_pocket.sln")))
        {
            return workingDir;
        }

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MushroomPocket");
    }

    /// <summary>
    /// Get save directory path
    /// </summary>
    public static string GetSaveDirPath()
    {
        return Path.Combine(GetDirPath(), "saves");
    }

    /// <summary>
    /// Ensure the save directory exists
    /// </summary>
    public static void EnsureSaveDir()
        => Directory.CreateDirectory(SaveDirPath);

    /// <summary>
    /// List all save files
    /// </summary>
    public static string[] ListSaveFiles()
        => Directory.EnumerateFiles(SaveDirPath, SaveFilePattern).ToArray();

    /// <summary>
    /// Create a new save file
    /// Returns the file path
    /// </summary>
    public static string CreateSaveFile(string name)
    {
        string filePath = Path.Combine(SaveDirPath, String.Format(NewSaveFilePattern, name));
        File.Copy(CurrentDbName, filePath);
        return filePath;
    }

    /// <summary>
    /// Get save file names
    /// </summary>
    public static string[] GetSaveNames()
        => ListSaveFiles().Select(s => Path.GetFileNameWithoutExtension(s)).ToArray();

    /// <summary>
    /// If save file exists
    /// </summary>
    public static bool SaveFileExists(string filePath)
        => ListSaveFiles().Contains(filePath);

    /// <summary>
    /// Get file path from name
    /// </summary>
    public static string GetFilePathFromName(string name)
        => ListSaveFiles().First(s => Path.GetFileNameWithoutExtension(s) == name);

    /// <summary>
    /// Use safe file
    /// </summary>
    public static void UseSafeFile(string filePath)
    {
        if (!SaveFileExists(filePath))
            throw new FileNotFoundException($"Save file not found: {filePath}");

        // Restore DB from save file
        File.Delete(CurrentDbName);
        File.Copy(Path.Combine(SaveDirPath, filePath), CurrentDbName);
    }

    /// <summary>
    /// Delete save file
    /// </summary>
    public static void DeleteSaveFile(string filePath)
        => DeleteSaveFiles(new[] { filePath });

    /// <summary>
    /// Delete save files
    /// </summary>
    public static void DeleteSaveFiles(IEnumerable<string> filePaths)
    {
        HashSet<string> setFilePaths = filePaths.ToHashSet();
        string[] saveFiles = ListSaveFiles();

        if (setFilePaths.Count != setFilePaths.Intersect(saveFiles).Count())
            throw new FileNotFoundException($"Some files not found: {string.Join(", ", setFilePaths.Except(saveFiles))}");

        foreach (string filePath in setFilePaths)
        {
            File.Delete(filePath);
        }
    }
}
