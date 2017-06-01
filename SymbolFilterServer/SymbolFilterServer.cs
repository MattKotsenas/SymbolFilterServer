using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SymbolFilterServer
{
    // ReSharper disable once ClassNeverInstantiated.Global -- Instantiated by middleware
    public class SymbolFilterServer
    {
        private readonly RequestDelegate _next;

        public SymbolFilterServer(RequestDelegate next)
        {
            _next = next;
        }

        // ReSharper disable once UnusedMember.Global -- Called by middleware
        public async Task Invoke(HttpContext context, Arguments args, RedirectParser parser)
        {
            var symbolFilterList = args.Symbols;
            if (context.Request.Method != "GET")
            {
                Respond404(context.Response);
            }

            // Avoid case sensitivity issues for matching the pattern
            var path = context.Request.Path.ToString();
            path = Uri.UnescapeDataString(path).ToLowerInvariant();

            foreach (var symbol in symbolFilterList)
            {
                if (path.Contains(symbol))
                {
                    Console.WriteLine("Matched pattern: {0}", symbol);
                    MaybeRedirect(context.Response, parser, path);
                    return;
                }
            }
            Respond404(context.Response);

            await _next(context);
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

        private void MaybeRedirect(HttpResponse response, RedirectParser parser, string req)
        {
            var result = parser.Parse(req);

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