using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

[assembly: ASNA.QSys.Expo.Model.ExpoModelAssembly()] // Mark this assembly as containing Display Page Models.

namespace ExpoSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var encodingsBefore = System.Text.Encoding.GetEncodings();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<CustomerAppSite.Startup>();
                });
    }
}

