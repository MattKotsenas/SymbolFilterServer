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
        private readonly RedirectParser _redirectParser = new RedirectParser();

        // The allowed PDBs we do *not* ignore
        private readonly IReadOnlyList<string> _dllFilterList; // TODO: Move this to IEnumerable

        public SymbolFilterServer(Arguments args)
        {
            _args = args;
            _dllFilterList = _args.Symbols.ToList();
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
                var request = context.Request;
                var req = request.Path;

                // avoid case sensitivity issues for matching the pattern
                var reqLower = Uri.UnescapeDataString(req).ToLowerInvariant();

                int i;
                for (i = 0; i < _dllFilterList.Count; i++)
                {
                    if (reqLower.Contains(_dllFilterList[i]))
                        break;
                }

                // if we didn't match, or it isn't a GET then serve up a 404
                if (i == _dllFilterList.Count || request.Method != "GET")
                {
                    // you don't match, fast exit, this is basically the whole point of this thing
                    Return404(context);
                }
                else
                {
                    // this is the real work
                    Console.WriteLine("Matched pattern: {0}", _dllFilterList[i]);
                    RedirectRequest(context, req);
                }
            });
        }

        // cons up a minimal 404 error and return it
        private void Return404(HttpContext context)
        {
            // it doesn't get any simpler than this
            context.Response.StatusCode = 404;
        }

        private void Return302(HttpContext context, string url)
        {
            Console.WriteLine("302 Redirect {0}", url);

            // emit the redirect
            context.Response.Redirect(url, false);
        }

        private void RedirectRequest(HttpContext context, string req)
        {
            var result = _redirectParser.Parse(req);

            if (result.IsValid)
            {
                Return302(context, result.Redirect);
            }
            else
            {
                Return404(context);
            }
        }
    }
}