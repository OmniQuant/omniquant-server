using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace HelloWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder().UseKestrel().UseIISIntegration().UseWebRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build();
            host.Run();
        }
    }
}