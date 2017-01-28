using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MyExplorerMVC.Models
{
    public class MyFile : IFile
    {
        public string Name { get; set; }
        public string FullPath { get; set; }

        public int Depth { get { return FullPath.Split(Path.DirectorySeparatorChar).Length; } }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateModified { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }       
    }
}
