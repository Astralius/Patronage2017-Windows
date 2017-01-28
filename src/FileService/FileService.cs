using System;
using System.Collections.Generic;
using System.IO;

namespace Explorer.Services
{
    public static class FileService
    {
        /// <summary>
        /// Returns information about all files in the specified directory and all subdirectories.
        /// </summary>
        /// <param name="path">Location (path) of the directory to enumerate.</param>
        /// <returns>A list with files' information or null if something went wrong.</returns>
        public static List<FileInfo> GetFiles(string path, bool topDirectoryOnly = false)
        {
            // Validation: exists = false, when path is invalid -or- directory does not exist -or- the user does not have read rights for the directory.
            if (!Directory.Exists(path)) return null;

            var entries = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
            var items = new List<FileInfo>();

            foreach (string entry in entries)
            {
                bool isDirectory = (File.GetAttributes(entry) & FileAttributes.Directory) == FileAttributes.Directory;

                #region Files

                if (!isDirectory)
                {
                    items.Add(new FileInfo(entry));
                    continue;
                }

                #endregion

                #region Directories

                if (isDirectory && !topDirectoryOnly)
                {
                    // recursive call to sort the resulting files in the right order (depth first)
                    items.AddRange(GetFiles(entry));
                }

                #endregion
            }

            return items;
        }

        /// <summary>
        /// Returns information about the file under the specified path.
        /// </summary>
        /// <param name="path">Location (path) of the file.</param>
        /// <returns>A MyFile object containing metadata of the file or null if something went wrong.</returns>
        public static MyFile GetFileInfo(string path)
        {
            // Validation: exists = false, when path is invalid -or- file does not exist -or- the user does not have read rights for the file.
            if (!File.Exists(path)) return null;

            FileInfo info = new FileInfo(path);
            MyFile file = new MyFile
            {
                Name = info.Name,
                FullPath = info.FullName,
                DateCreated = info.CreationTime,
                DateModified = info.LastWriteTime
            };
            return file;
        }
    }
}
