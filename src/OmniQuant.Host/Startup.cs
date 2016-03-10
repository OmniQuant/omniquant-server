using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace OmniQuant.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IApplicationEnvironment applicationEnvironment)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .AddCommandLine(new string[0]);
            Configuration = configBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddOptions();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");
        }
    }
}