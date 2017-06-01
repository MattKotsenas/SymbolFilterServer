using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SymbolFilterServer
{
    public class Startup : IStartup
    {
        private readonly Arguments _arguments;

        public Startup(Arguments arguments)
        {
            _arguments = arguments;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<RedirectParser>();
            services.AddSingleton(_arguments);
            services.AddLogging();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<SymbolFilterServer>();
        }
    }
}