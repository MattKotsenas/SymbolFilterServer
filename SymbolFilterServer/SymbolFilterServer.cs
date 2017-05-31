using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SymbolFilterServer
{
    internal class SymbolFilterServer
    {
        private readonly Arguments _args;
        private readonly IEnumerable<string> _symbolFilterList; // The allowed PDBs we do *not* ignore
        private readonly RedirectParser _redirectParser;


        public SymbolFilterServer(Arguments args, RedirectParser parser)
        {
            _args = args;
            _symbolFilterList = _args.Symbols.ToList();
            _redirectParser = parser;
        }

        internal void Run()
        {
            new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(Request))
                .UseUrls($"http://localhost:{_args.Port}")
                .Build()
                .Run();
        }

        private async Task Request(HttpContext context)
        {
            await Task.Run(() =>
            {
                if (context.Request.Method != "GET") { Respond404(context.Response); }

                var path = context.Request.Path.ToString();

                // Avoid case sensitivity issues for matching the pattern
                path = Uri.UnescapeDataString(path).ToLowerInvariant();

                foreach (var symbol in _symbolFilterList)
                {
                    if (path.Contains(symbol))
                    {
                        Console.WriteLine("Matched pattern: {0}", symbol);
                        MaybeRedirect(context.Response, path);
                        return;
                    }
                }
                Respond404(context.Response);
            });
        }

        private void Respond404(HttpResponse response)
        {
            response.StatusCode = 404;
        }

        private void Respond302(HttpResponse response, string url)
        {
            Console.WriteLine("302 Redirect {0}", url);
            response.Redirect(url, false);
        }

        private void MaybeRedirect(HttpResponse response, string req)
        {
            var result = _redirectParser.Parse(req);

            if (result.IsValid)
            {
                Respond302(response, result.Redirect);
            }
            else
            {
                Respond404(response);
            }
        }
    }
}