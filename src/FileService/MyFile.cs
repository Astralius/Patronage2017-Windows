using System;

namespace Explorer.Services
{
    public class MyFile : IFile
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
