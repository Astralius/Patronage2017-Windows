using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Explorer.Services;

namespace MyExplorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsoleColor startingColor = Console.ForegroundColor;

            if (args.Length < 1  ) return;
            string path = args.First();

            // How deep in the file system is the chosen path
            int rootDepth = path.Split(Path.DirectorySeparatorChar).Length;
            // How wide is the current console window
            int windowWidth = Console.WindowWidth;

            // Decorations            
            DisplayHeader(ConsoleColor.Green);

            // Core: Get all files under the chosen path
            var files = FileService.GetFiles(path);

            // Core: Get details of each file and display them
            foreach (var file in files)
            {
                MyFile info = FileService.GetFileInfo(file.FullName);
                DisplayFile(info, rootDepth, windowWidth);
            }

            // Decorations
            DisplayHorizontalLine(ConsoleColor.Green, '=');
            Console.ForegroundColor = startingColor;
        }

        /// <summary>
        /// Displays file information to the console.
        /// </summary>
        /// <param name="info">Reference to file information.</param>
        private static void DisplayFile(MyFile file, int rootDepth, int windowWidth)
        {
            string itemLine = file.FullPath;
            int relativeDepth = file.Depth - rootDepth - 1;

            Console.ForegroundColor = ConsoleColor.White;

            if (relativeDepth > 0)
            {
                string[] parts;

                // Cross-platform split
                parts = itemLine.Split(Path.DirectorySeparatorChar);

                // Formatting for items in subfolders
                if (parts.Length > 1)
                {
                    itemLine = 
                        ' ' + 
                        parts[parts.Length - 2] + Path.DirectorySeparatorChar + 
                        parts.Last();
                }
            }

            // Extra indentation for files/directories in subfolders
            itemLine = itemLine.PadLeft(itemLine.Length + relativeDepth, '-');

            // Getting the date and time of the last modification of the file/directory
            string dateModified = file.DateModified.ToString(CultureInfo.CurrentCulture);

            // Shortening of too long paths to fit the dateModified string
            itemLine = itemLine.Truncate(windowWidth - dateModified.Length);

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
