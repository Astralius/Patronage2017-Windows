using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Explorer.Services;

namespace MyExplorerMVC
{
    public class MyFormatter
    {
        private const string TextHtmlUtf8 = "text/html; charset=utf-8";

        private readonly HtmlEncoder _htmlEncoder;

        public MyFormatter(HtmlEncoder encoder)
        {
            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }
            _htmlEncoder = encoder;
        }

        /// <summary>
        /// Generates an HTML view for a directory.
        /// </summary>
        public virtual Task ListFilesAsync(HttpContext context, IEnumerable<FileInfo> contents)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            context.Response.ContentType = TextHtmlUtf8;

            PathString requestPath = context.Request.PathBase + context.Request.Path;

            var builder = new StringBuilder();

            builder.AppendFormat(@"
                <!DOCTYPE html>
                <html lang=""{0}"">", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            builder.AppendFormat(@"
                <head>
                    <title>{0} {1}</title>", "Pliki w katalogu ", HtmlEncode(requestPath.Value));

            builder.Append(@"
                    <style>
                        body {
                            font-family: ""Segoe UI"", ""Segoe WP"", ""Helvetica Neue"", 'RobotoRegular', sans-serif;
                            font-size: 14px;}
                        header h1 {
                            font-family: ""Segoe UI Light"", ""Helvetica Neue"", 'RobotoLight', ""Segoe UI"", ""Segoe WP"", sans-serif;
                            font-size: 28px;
                            font-weight: 100;
                            margin-top: 5px;
                            margin-bottom: 0px;}
                        #index {
                            border-collapse: separate; 
                            border-spacing: 0; 
                            margin: 0 0 20px; }
                        #index th {
                            vertical-align: bottom;
                            padding: 10px 5px 5px 5px;
                            font-weight: 400;
                            color: #a0a0a0;
                            text-align: left; }
                        #index td { padding: 3px 10px; }
                        #index th, #index td {
                            border-right: 1px #ddd solid;
                            border-bottom: 1px #ddd solid;
                            border-left: 1px transparent solid;
                            border-top: 1px transparent solid;
                            box-sizing: border-box; }
                        #index th:last-child, #index td:last-child {
                            border-right: 1px transparent solid; }
                        #index td.name, td.path { text-align:left; }
                        a { color:#1ba1e2;text-decoration:none; }
                        a:hover { color:#13709e;text-decoration:underline; }
                    </style>
                </head>
                <body>
                    <section id=""main"">");

            builder.AppendFormat(@"
                    <header><h1>{0} <a href=""/"">/</a>", "Pliki w katalogu ");

            builder.AppendFormat(CultureInfo.CurrentUICulture, @"
                    </h1></header>
                    <table id=""index"" summary=""{0}"">
                        <thead>
                            <tr>
                                <th abbr=""{1}"">{1}</th>
                            </tr>
                        </thead>
                        <tbody>",
                    "Lista plikow w wybranym katalogu",
                    "Nazwa");

            foreach (var file in contents)
            {
                builder.AppendFormat(@"
                            <tr class=""file"">
                                <td class=""name""><a href=""./{0}"">{0}</a></td>
                            </tr>",
                    HtmlEncode(file.Name));
            }

            builder.Append(@"
                        </tbody>
                    </table>
                    </section>
                </body>
                </html>");

            string data = builder.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            context.Response.ContentLength = bytes.Length;
            return context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Generates an HTML view for a single file.
        /// </summary>
        public virtual Task ListFileMetadataAsync(HttpContext context, MyFile contents)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            context.Response.ContentType = TextHtmlUtf8;

            PathString requestPath = context.Request.PathBase + context.Request.Path;

            var builder = new StringBuilder();

            builder.AppendFormat(@"
                <!DOCTYPE html>
                <html lang=""{0}"">", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            builder.AppendFormat(@"
                <head>
                    <title>{0} {1}</title>", "Informacje o pliku ", contents.Name);

            builder.Append(@"
                    <style>
                        body {
                            font-family: ""Segoe UI"", ""Segoe WP"", ""Helvetica Neue"", 'RobotoRegular', sans-serif;
                            font-size: 14px;}
                        header h1 {
                            font-family: ""Segoe UI Light"", ""Helvetica Neue"", 'RobotoLight', ""Segoe UI"", ""Segoe WP"", sans-serif;
                            font-size: 28px;
                            font-weight: 100;
                            margin-top: 5px;
                            margin-bottom: 0px;}
                        #index {
                            border-collapse: separate; 
                            border-spacing: 0; 
                            margin: 0 0 20px; }
                        #index th {
                            vertical-align: bottom;
                            padding: 10px 5px 5px 5px;
                            font-weight: 400;
                            color: #a0a0a0; }                            
                        #index td { padding: 3px 10px; }
                        #index th, #index td {
                            border-right: none;
                            border-bottom: none;
                            border-left: none;
                            border-top: none;
                            box-sizing: border-box; }
                        #index th.nameHeader, th.pathHeader { text-align:left; }
                        #index th.createdHeader, th.modifiedHeader { text-align:right };
                        #index td.created, td.modified { text-align:right; }
                        a { color:#1ba1e2;text-decoration:none; }
                        a:hover { color:#13709e;text-decoration:underline; }
                    </style>
                </head>
                <body>
                    <section id=""main"">");

            builder.AppendFormat(@"
                    <header><h1>{0} <a href=""/"">/</a>", "Plik ", requestPath);

            string cumulativePath = "/";
            foreach (var segment in requestPath.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                cumulativePath = cumulativePath + segment + "/";
                builder.AppendFormat(@"<a href=""{0}"">{1}/</a>",
                    HtmlEncode(cumulativePath), HtmlEncode(segment));
            }

            builder.AppendFormat(CultureInfo.CurrentUICulture, @"
                    </h1></header>
                    <table id=""index"" summary=""{0}"">
                        <thead>
                            <tr>
                                <th class=""nameHeader"" abbr=""{1}"">{1}</th>
                                <th class=""pathHeader"" abbr=""{2}"">{2}</th>
                                <th class=""createdHeader"" abbr=""{3}"">{3}</th>
                                <th class=""modifiedHeader"" abbr=""{4}"">{4}</th>
                            </tr>
                        </thead>
                        <tbody>",
                    "Szczegółowe informacje o wybranym pliku.",
                    "Nazwa",
                    "Sciezka",
                    "Data Utworzenia",
                    "Data Modyfikacji");

            builder.AppendFormat(@"
                        <tr class=""file"">
                            <td class=""name"">{0}</td>
                            <td class=""path"">{1}</td>
                            <td class=""created"">{2}</td>
                            <td class=""modified"">{3}</td>
                        </tr>",
                HtmlEncode(contents.Name),
                HtmlEncode(contents.FullPath),
                HtmlEncode(contents.DateCreated.ToString(CultureInfo.CurrentCulture)),
                HtmlEncode(contents.DateModified.ToString(CultureInfo.CurrentCulture)));

            builder.Append(@"
                        </tbody>
                    </table>
                    </section>
                </body>
                </html>");

            string data = builder.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            context.Response.ContentLength = bytes.Length;
            return context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        private string HtmlEncode(string body)
        {
            return _htmlEncoder.Encode(body);
        }
    }
}