using System.IO;

namespace SerilogTest2
{
    internal static class PathHelpers
    {
        /// <summary>
        /// Combine paths
        /// </summary>
        /// <param name="path">basepath, not null</param>
        /// <param name="dir">optional dir</param>
        /// <param name="file">optional file</param>
        /// <returns></returns>
        internal static string CombinePaths(string path, string dir, string file)
        {
            if (dir != null)
            {
                path = Path.Combine(path, dir);
            }

            if (file != null)
            {
                path = Path.Combine(path, file);
            }
            return path;
        }

        /// <summary>
        /// Cached directory separator char array to avoid memory allocation on each method call.
        /// </summary>
        private static readonly char[] DirectorySeparatorChars = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        /// <summary>
        /// Trims directory separators from the path
        /// </summary>
        /// <param name="path">path, could be null</param>
        /// <returns>never null</returns>
        public static string TrimDirectorySeparators(string path)
        {
            return path?.TrimEnd(DirectorySeparatorChars) ?? string.Empty;
        }

        public static bool IsTempDir(string directory, string tempDir)
        {
            tempDir = TrimDirectorySeparators(tempDir);
            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(tempDir))
                return false;

            var fullpath = Path.GetFullPath(directory);
            if (string.IsNullOrEmpty(fullpath))
                return false;

            if (fullpath.StartsWith(tempDir, System.StringComparison.OrdinalIgnoreCase))
                return true;

            if (tempDir.StartsWith("/tmp") && directory.StartsWith("/var/tmp/"))
                return true;    // Microsoft has made a funny joke on Linux. Path.GetTempPath() uses /tmp/ as fallback, but single-publish uses /var/tmp/ as first fallback

            return false;
        }
    }
}
