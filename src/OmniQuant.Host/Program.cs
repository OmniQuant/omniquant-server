using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace OmniQuant.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                        .UseServer("Microsoft.AspNetCore.Server.Kestrel")
                        .UseApplicationBasePath(Directory.GetCurrentDirectory())
                        .UseDefaultConfiguration(args)
                        .UseStartup<Startup>()
                        .Build();

            host.Run();
        }
    }
}