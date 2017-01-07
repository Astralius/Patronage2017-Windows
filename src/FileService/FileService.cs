using System;
using System.Collections.Generic;
using System.IO;

namespace Explorer.Services
{
    public class FileService
    {
        /// <summary>
        /// Returns information about all files and folders in the specified directory.
        /// </summary>
        /// <param name="path">Location (path) of the directory to enumerate.</param>
        /// <returns>A list with files' information or null if something went wrong.</returns>
        public List<FileSystemInfo> GetFiles(string path)
        {
            // Walidacja dostępu: exists = false, gdy ścieżka jest niepoprawna -lub- katalog nie istnieje -lub- użytkownik nie ma prawa odczytu.
            if (!Directory.Exists(path)) return null;

            IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
            List<FileSystemInfo> items = new List<FileSystemInfo>();

            foreach (string entry in entries)
            {
                bool isDirectory = (File.GetAttributes(entry) & FileAttributes.Directory) == FileAttributes.Directory;
                FileSystemInfo info;

                #region Files

                if (!isDirectory)
                {
                    info = new FileInfo(entry);
                    items.Add(info);
                }

                #endregion

                #region Directories

                if (isDirectory)
                {
                    info = new DirectoryInfo(entry);
                    items.Add(info);

                    // recursive call to sort the resulting files in the right order (depth first)
                    items.AddRange(GetFiles(info.FullName));
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
        public MyFile GetFileInfo(string path)
        {
            // Walidacja dostępu: exists = false, gdy ścieżka jest niepoprawna -lub- katalog nie istnieje -lub- użytkownik nie ma prawa odczytu.
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
