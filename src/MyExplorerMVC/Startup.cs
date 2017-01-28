using System.IO;
using System.Text.Encodings.Web;
using Explorer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace MyExplorerMVC
{
    public class Startup
    {
        private MyFormatter formatter;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddDirectoryBrowser();
            formatter = new MyFormatter(HtmlEncoder.Default);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var routeBuilder = new RouteBuilder(app);

            // Default behaviour: display a list of all files in the project web root
            routeBuilder.MapGet("",
                context => formatter.ListFilesAsync(
                    context, FileService.GetFiles(env.WebRootPath, true)));

            // Display file based on its typed name (or clicked link)
            routeBuilder.MapGet("{fileName}",
                context => formatter.ListFileMetadataAsync(
                                context, FileService.GetFileInfo(
                                    env.WebRootPath + Path.DirectorySeparatorChar + context.GetRouteValue("fileName").ToString())));

            app.UseRouter(routeBuilder.Build());
        }
    }
}