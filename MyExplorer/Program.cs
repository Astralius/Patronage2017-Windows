using System;
using System.Collections.Generic;
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

            // Domyślnym zachowaniem jest wyświetlenie samych plików. Zgodnie ze specyfikacją (Krok 3): 
            // "- dokonała enumeracji wszystkich plików w folderze i wypisała ich nazwy na konsoli wraz ze ścieżką"
            // Opcja: Jeżeli autor miał na myśli wszystkie pliki oraz foldery, można wywołać EnumerateContents(path, true).
            // Opcja: Jeżeli autor miał na myśli wszystkie pliki z uwzględnieniem podfolderów i ich zawartości, można wywołać EnumerateContents(path, true, true).
            EnumerateContents(path);                               
        }

        /// <summary>
        /// Lists the contents of the directory from a given, valid path.
        /// </summary>
        /// <param name="path">Location (path) of the directory to enumerate.</param>
        /// <param name="showDirectories">Should the listing include directories (folders)?</param>
        /// <param name="includeSubfolders">Should the listing also include subdirectories and their contents?</param>
        public static void EnumerateContents(string path, bool showDirectories = false, bool includeSubfolders = false)
        {
            IEnumerable<string> entries = (includeSubfolders)
                ? Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories)
                : Directory.EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);

            foreach (string entry in entries)
            {
                bool isDirectory = (File.GetAttributes(entry) & FileAttributes.Directory) == FileAttributes.Directory;
                if (isDirectory && !showDirectories) continue;
                Console.WriteLine(entry);
            }
        }
    }
}
