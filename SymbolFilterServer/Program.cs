using Microsoft.AspNetCore.Hosting;

namespace SymbolFilterServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Arguments = new ArgumentsParser().Parse(args);

            new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls($"http://localhost:{Arguments.Port}")
                .Build()
                .Run();
        }

        public static Arguments Arguments { get; private set; } // TODO: Can this be passed into the WebHostBuilder?
    }
}