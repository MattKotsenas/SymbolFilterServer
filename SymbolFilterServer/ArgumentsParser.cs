using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace SymbolFilterServer
{
    internal class ArgumentsParser
    {
        public Arguments Parse(string[] args)
        {
            var port = 8080;
            IReadOnlyList<string> symbols = new List<string>();
            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("p|port", ref port, $"The port to listen on (defaults to {port}");
                syntax.DefineParameterList("symbols", ref symbols, "The symbols to proxy. All other symbols will return '404 Not Found'");

                symbols = symbols.Select(s => s.Trim().ToLowerInvariant()).ToList();
            });

            return new Arguments(port, symbols);
        }
    }

    internal class Arguments
    {
        public Arguments(int port, IEnumerable<string> symbols)
        {
            Port = port;
            Symbols = symbols;
        }

        public int Port { get; }
        public IEnumerable<string> Symbols { get; }
    }
}
