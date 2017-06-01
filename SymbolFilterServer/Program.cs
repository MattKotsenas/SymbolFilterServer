using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SymbolFilterServer
{
    // ReSharper disable once UnusedMember.Global -- Called by CLR
    public class Program
    {
        // ReSharper disable once UnusedMember.Global -- Called by CLR
        public static void Main(string[] args)
        {
            var arguments = new ArgumentsParser().Parse(args);

            new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IStartup>(new Startup(arguments));
                })
                .UseUrls($"http://localhost:{arguments.Port}")
                .ConfigureLogging(factory => factory.AddConsole())
                .Build()
                .Run();
        }
    }
}