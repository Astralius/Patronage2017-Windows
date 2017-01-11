using System;
using System.IO;

namespace Explorer.Services
{
    public class MyFile : IFile
    {
        public string Name { get; set; }
        public string FullPath { get; set; }

        public int Depth { get { return FullPath.Split(Path.DirectorySeparatorChar).Length; } }

        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }       
    }
}
