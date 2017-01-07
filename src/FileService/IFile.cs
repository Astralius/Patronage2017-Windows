using System;

namespace Explorer.Services
{
    interface IFile
    {
        string Name { get; set; }
        string FullPath { get; set; }
        DateTime DateModified { get; set; }
        DateTime DateCreated { get; set; }
    }
}
