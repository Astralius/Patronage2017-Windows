using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


namespace MyExplorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1) return;
            string path = args.First();

            // Walidacja dostępu: exists = false, gdy ścieżka jest niepoprawna -lub- katalog nie istnieje -lub- użytkownik nie ma prawa odczytu.
            bool exists = Directory.Exists(path);

            Console.WriteLine(exists ? "Katalog istnieje." : "Katalog nie istnieje");  
            if(!exists) return;

            ConsoleColor startingColor = Console.ForegroundColor;
            DisplayHeader(ConsoleColor.Green);

            // Domyślnym zachowaniem jest wyświetlenie samych plików. Zgodnie ze specyfikacją (Krok 3): 
            // "- dokonała enumeracji wszystkich plików w folderze i wypisała ich nazwy na konsoli wraz ze ścieżką"
            EnumerateContents(path, args.Contains("-directories"), args.Contains("-subfolders"));      
            
            DisplayHorizontalLine(ConsoleColor.Green, '=');
            Console.ForegroundColor = startingColor;
        }

        /// <summary>
        /// Lists the contents of the directory from a given, valid path.
        /// </summary>
        /// <param name="path">Location (path) of the directory to enumerate.</param>
        /// <param name="showDirectories">Should the listing include directories (folders)?</param>
        /// <param name="includeSubfolders">Should the listing also include subdirectories and their contents?</param>
        /// <param name="level">Level in directory tree (affects indentation).</param>
        public static void EnumerateContents(string path, bool showDirectories = false, bool includeSubfolders = false, int level = 0)
        {
            IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
            
            foreach (string entry in entries)
            {
                bool isDirectory = (File.GetAttributes(entry) & FileAttributes.Directory) == FileAttributes.Directory;
                FileSystemInfo info;
               
                #region Files
                if (!isDirectory)
                {
                    info = new FileInfo(entry);
                    DisplayItem(info, level);
                }
                #endregion
                #region Directories
                if (isDirectory)
                {
                    info = new DirectoryInfo(entry);

                    if (showDirectories)
                    {
                        DisplayItem(info, level);
                    }

                    if (includeSubfolders)
                    {
                        // rekurencyjne wywołanie aby ustawić wyniki w odpowiedniej kolejności
                        EnumerateContents(info.FullName, showDirectories, true, level + 1);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Displays item line base on its type (file/folder) and level in hierarchy
        /// </summary>
        /// <param name="info">Item information reference.</param>
        /// <param name="indent">Line indentation from the left border of the console.</param>
        private static void DisplayItem(FileSystemInfo info, int indent)
        {
            string itemLine = info.FullName;
            int width = Console.WindowWidth;

            if (!(info is FileInfo) && !(info is DirectoryInfo))
            {
                // throw new ArgumentException("Unsupported file system item type at item: ", info.Name);
                Console.ForegroundColor = ConsoleColor.Red;
                itemLine = "Unsupported item: " + info.Name;
                itemLine = itemLine.PadLeft(indent);
                itemLine = itemLine.Truncate(width);
                Console.WriteLine(itemLine);
                return;
            }
            
            if (info is FileInfo)
            {
                Console.ForegroundColor = ConsoleColor.Gray;              
            }

            if (info is DirectoryInfo)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (indent > 0)
            {
                string[] parts;

                // Unix
                parts = itemLine.Split('/');
                if (parts.Length > 1) itemLine = ' ' + parts[parts.Length - 2] + '/' + parts.Last();

                // Windows
                parts = itemLine.Split('\\');
                if (parts.Length > 1) itemLine = ' ' + parts[parts.Length - 2] + '\\' + parts.Last();
            }

            // Dodatkowe wcięcie dla rzeczy w podfolderach
            itemLine = itemLine.PadLeft(itemLine.Length + indent, '-');

            // Pobieram datę i czas ostatniej modyfikacji pliku
            string dateModified = info.LastWriteTime.ToString(CultureInfo.CurrentCulture);

            // Skracam zbyt długie ścieżki aby zmiecić dateModified
            itemLine = itemLine.Truncate(width - dateModified.Length);

            Console.WriteLine(itemLine + dateModified);
        }

        /// <summary>
        /// Displays a fancy header for the listing results.
        /// </summary>
        /// <param name="col">Color of the header.</param>
        private static void DisplayHeader(ConsoleColor col)
        {
            DisplayHorizontalLine(col, '=');

            Console.ForegroundColor = col;
            string t1 = "File:";
            string t2 = "Last Modified:     ";
            Console.WriteLine(t1.Truncate(Console.WindowWidth - t2.Length) + t2);

            DisplayHorizontalLine(col, '=');
        }

        /// <summary>
        /// Displays a fancy decorative horizontal line.
        /// </summary>
        /// <param name="col">Color of the header.</param>
        /// <param name="c">Symbol (character) to use for drawing the line.</param>
        private static void DisplayHorizontalLine(ConsoleColor col, char c)
        {
            Console.ForegroundColor = col;
            Console.WriteLine(string.Empty.PadRight(Console.WindowWidth, c));
        }
    }
}
