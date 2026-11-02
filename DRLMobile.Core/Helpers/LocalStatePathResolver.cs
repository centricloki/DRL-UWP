using System;
using System.IO;

namespace DRLMobile.Core.Helpers 
{
    public static class LocalStatePathResolver
    {
        /// <summary>
        /// Global property that the UWP App can set ONCE at startup.
        /// This allows library functions to use it without passing the path every time.
        /// </summary>
        public static string CurrentLocalStatePath { get; set; }

        /// <summary>
        /// Resolves the file path using the globally set CurrentLocalStatePath.
        /// </summary>
        public static string Resolve(string oldFilePath)
        {
            return ResolveFilePath(CurrentLocalStatePath, oldFilePath);
        }

        /// <summary>
        /// Pure static function to replace the old path with a new one.
        /// Safe to use anywhere in the shared library.
        /// </summary>
        /// <param name="newLocalStatePath">The new app's LocalState path (passed from UWP app)</param>
        /// <param name="oldFilePath">The old file path string retrieved from SQLite</param>
        /// <returns>The corrected file path, or the original string if it's a URL or invalid</returns>
        public static string ResolveFilePath(string newLocalStatePath, string oldFilePath)
        {
            // 1. Basic null/empty checks
            if (string.IsNullOrWhiteSpace(oldFilePath) || string.IsNullOrWhiteSpace(newLocalStatePath))
            {
                return oldFilePath;
            }

            // 2. SKIP: If it's a web URL (http:// or https://)
            if (oldFilePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                oldFilePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return oldFilePath;
            }

            // 3. CHECK: Must be a local drive path (e.g., "C:\" or "D:/")
            // Ensures the string is at least 3 chars, starts with a letter, followed by ':' and '\' or '/'
            bool isLocalDrive = oldFilePath.Length >= 3 &&
                                char.IsLetter(oldFilePath[0]) &&
                                oldFilePath[1] == ':' &&
                                (oldFilePath[2] == '\\' || oldFilePath[2] == '/');

            if (!isLocalDrive)
            {
                return oldFilePath; // Skip UNC paths (\\server\share) or relative paths
            }

            // 4. FIND: Locate "\LocalState\" or "/LocalState/" (case-insensitive)
            int localStateIndex = oldFilePath.IndexOf("\\LocalState\\", StringComparison.OrdinalIgnoreCase);
            if (localStateIndex == -1)
            {
                localStateIndex = oldFilePath.IndexOf("/LocalState/", StringComparison.OrdinalIgnoreCase);
            }

            if (localStateIndex != -1)
            {
                // The marker "\LocalState\" or "/LocalState/" is exactly 12 characters long
                int markerLength = 12;

                // Extract everything AFTER the marker 
                // Example: "Activity\44fcc90d-61c3-4f99-9178-17d35c148849_Narrative-WeeklyReport_20261030_105409.png"
                string relativePath = oldFilePath.Substring(localStateIndex + markerLength);

                // Safely combine the new base path with the relative file path
                return Path.Combine(newLocalStatePath, relativePath);
            }

            // 5. FALLBACK: If "LocalState" isn't found in the local path, return original
            return oldFilePath;
        }
    }
}