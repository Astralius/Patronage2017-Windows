using System.IO;
using System.Text.Encodings.Web;
using Explorer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace MyExplorerMVC
{
    public class Startup
    {
        private MyFormatter formatter;

        public void ConfigureServices(IServiceCollection services)
        {
            formatter = new MyFormatter(HtmlEncoder.Default);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(context => formatter.ListFilesAsync(
                    context, FileService.GetFiles(env.ContentRootPath, true)));
        }
    }
}
