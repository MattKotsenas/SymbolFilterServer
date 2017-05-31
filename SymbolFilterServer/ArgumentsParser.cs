using System.CommandLine;

namespace SymbolFilterServer
{
    class ArgumentsParser
    {
        public Arguments Parse(string[] args)
        {
            var port = 8080;
            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("p|port", ref port, $"The port to listen on (defaults to {port}");

            });

            return new Arguments(port);
        }
    }

    class Arguments
    {
        public Arguments(int port)
        {
            Port = port;
        }

        public int Port { get; }
    }
}
