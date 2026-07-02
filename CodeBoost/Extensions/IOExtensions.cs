#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBoost.Logging;
using CodeBoost.Types;

namespace CodeBoost.Extensions;

public static class IoExtensions
{
    /// <summary>
    /// Specifies how to format a platform path.
    /// </summary>
    public enum PathFormattingType : byte
    {
        /// <summary>
        /// Do not format the path.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Formats the path for the current platform type.
        /// </summary>
        FormatToPlatform = 1
    }

    /// <summary>
    /// Specifies how to write data to a file.
    /// </summary>
    public enum WriteType : byte
    {
        /// <summary>
        /// Appends onto current data.
        /// </summary>
        Append = 0,
        /// <summary>
        /// Replaces existing data with new data.
        /// </summary>
        Create = 1
    }

    /// <summary>
    /// Writes a text value to the specified file path.
    /// </summary>
    public static void WriteToFile(string value, string path, WriteType writeType = WriteType.Create, PathFormattingType pathFormattingType = PathFormattingType.FormatToPlatform)
    {
        //Path is not valid.
        if (string.IsNullOrWhiteSpace(path))
        {
            Logger.LogError($"{nameof(WriteToFile)}: path is null or empty.");
            return;
        }

        // If to format the path for the platform.
        if (pathFormattingType == PathFormattingType.FormatToPlatform)
            path = FormatPlatformPath(path);

        // Get directory path.
        string? directory = Path.GetDirectoryName(path);

        if (directory is null)
        {
            Logger.LogError($"{nameof(WriteToFile)}: directory returned null for path [{path}].");
            return;
        }

        try
        {
            // If directory doesn't exist try to create it.
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
        catch (Exception ex)
        {
            Logger.LogError($"{nameof(WriteToFile)}: An error occurred while creating a directory. Error [{ex.Message}] Directory [{directory}].");
            return;
        }

        FileStream? fileStream = null;
        StreamWriter? writer = null;

        try
        {
            FileMode fileMode = writeType == WriteType.Append ? FileMode.Append : FileMode.Create;
            fileStream = new(path, fileMode, FileAccess.Write);

            writer = new(fileStream);

            writer.Write(value);
        }
        catch (Exception ex)
        {
            Logger.LogError($"{nameof(WriteToFile)}: An error occurred while writing to a file. Error [{ex.Message}] Path [{path}].");
        }
        finally
        {
            writer?.Dispose();
            fileStream?.Dispose();
        }
    }

    /// <summary>
    /// Formats a file path for the current platform.
    /// </summary>
    public static string FormatPlatformPath(string path)
    {
        //Path is not valid.
        if (string.IsNullOrWhiteSpace(path))
        {
            Logger.LogError($"{nameof(WriteToFile)}: path is null or empty.");
            return string.Empty;
        }

        try
        {
            // Normalize to platform separator
            string normalizedPath = Path.GetFullPath(path);
            return normalizedPath;
        }
        catch (Exception ex)
        {
            Logger.LogError($"{nameof(FormatPlatformPath)}: An error occurred while formatting a path. Error [{ex.Message}] Path [{path}].");
            return string.Empty;
        }
    }

    /// <summary>
    /// Returns files on a path which match the specified search pattern.
    /// </summary>
    public static List<string> GetDirectoryFilesRecursively(string path, string searchPattern) => GetDirectoryFilesRecursively(path, searchPattern, excludedPaths: null);

    /// <summary>
    /// Returns files on a path which match the specified search pattern.
    /// </summary>
    public static List<string> GetDirectoryFilesRecursively(string path, string searchPattern, List<string>? excludedPaths)
    {
        List<string> allFiles = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories).ToList();

        //Remove any files in excluded paths.
        if (excludedPaths?.Count > 0)
        {
            for (int i = 0; i < allFiles.Count; i++)
            {
                string file = allFiles[i];

                string p = Path.GetFullPath(file);

                foreach (string ep in excludedPaths)
                {
                    //Is in an excluded directory.
                    if (ep.Equals(p, StringComparison.InvariantCultureIgnoreCase))
                    {
                        allFiles.RemoveAt(i);
                        i--;

                        break;
                    }
                }
            }
        }

        return allFiles;
    }
}