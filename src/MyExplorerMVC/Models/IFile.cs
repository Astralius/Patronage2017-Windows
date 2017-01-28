using System;

namespace MyExplorerMVC.Models
{
    interface IFile
    {
        string Name { get; set; }
        string FullPath { get; set; }
        int Depth { get; }
        DateTime DateModified { get; set; }
        DateTime DateCreated { get; set; }
    }
}
