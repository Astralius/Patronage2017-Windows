using System.IO;
using Explorer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MyExplorerMVC.Controllers
{
    public class FilesController : Controller
    {
        private readonly IHostingEnvironment environment;

        public FilesController(IHostingEnvironment env)
        {
            environment = env;
        }

        // GET: /
        // GET: /Files/
        // GET: /Files/Index
        public IActionResult Index()
        {
            var files = FileService.GetFiles(environment.WebRootPath, true);
            if (files == null)
            {
                return NotFound();
            }

            return View(files);
        }

        // GET: /Files/Details/<filename>
        public IActionResult Details(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            string filePath = environment.WebRootPath + Path.DirectorySeparatorChar + name;
            var fileInfo = FileService.GetFileInfo(filePath);
            if (fileInfo == null)
            {
                return NotFound();
            }

            return Json(fileInfo);
        }
    }
}
