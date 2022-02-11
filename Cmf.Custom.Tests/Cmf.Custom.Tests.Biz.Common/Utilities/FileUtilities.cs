using System;
using System.IO;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class FileUtilities
    {
        /// <summary>
        /// Gets the file that was created:
        ///		- in the directory provided
        ///		- between the given time and current DataTime
        /// </summary>
        /// <param name="initialTime">The time the Download Actions button was pressed</param>
        /// <param name="path">The path to check if files were created</param>
        /// <param name="startsWith">File name starts with</param>
        /// <param name="endsWith">File name ends with</param>
        /// <returns>The file info</returns>
        public static FileInfo FileCreatedBetweenDates(DateTime initialTime, string path, string startsWith = null, string endsWith = null)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo result = dirInfo?
                .GetFiles()
                .FirstOrDefault(f => f.CreationTime >= initialTime && f.CreationTime <= DateTime.Now &&
                            (string.IsNullOrWhiteSpace(startsWith) || f.Name.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase)) &&
                            (string.IsNullOrWhiteSpace(endsWith) || f.Name.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase)));
            return result;
        }
    }
}
