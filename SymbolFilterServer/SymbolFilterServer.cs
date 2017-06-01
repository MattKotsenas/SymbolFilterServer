using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        public async Task Invoke(HttpContext context, Arguments args, RedirectParser parser, ILogger<SymbolFilterServer> logger)
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
                    logger.LogInformation("Matched pattern: {0}", symbol);
                    MaybeRedirect(context.Response, parser, logger, path);
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

        private void Respond302(HttpResponse response, ILogger logger, string url)
        {
            logger.LogInformation("302 redirect to {0}", url);
            response.Redirect(url, false);
        }

        private void MaybeRedirect(HttpResponse response, RedirectParser parser, ILogger logger, string url)
        {
            var result = parser.Parse(url);

            if (result.IsValid)
            {
                Respond302(response, logger, result.Redirect);
            }
            else
            {
                logger.LogError("Redirect url {0} was not valid. Returning 404", url);
                Respond404(response);
            }
        }
    }
}