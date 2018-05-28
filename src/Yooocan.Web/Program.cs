using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Yooocan.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = new WebHostBuilder()
                            .UseKestrel(options => options.AddServerHeader = false)
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseIISIntegration()
                            .UseStartup<Startup>()
                            .Build();
            return host;
        }
    }
}
