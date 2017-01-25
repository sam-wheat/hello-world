using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Net.Http;

namespace WebApplication1
{
    public class Program
    {
        private static Timer KeepAliveTimer;


        public static void Main(string[] args)
        {
            KeepAliveTimer = new Timer(x => WakeUpAPI(), null, 0, 5000);
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
            
        }


        private static void WakeUpAPI()
        {
            HttpClient client = new HttpClient();
            string dummy = client.GetStringAsync(new Uri("http://www.samwheat.com/api/api/blog/GetContentItemBySlug?slug=x&siteID=9999")).Result;
        }

        ~Program()
        {
            KeepAliveTimer.Dispose();
        }
    }
}
