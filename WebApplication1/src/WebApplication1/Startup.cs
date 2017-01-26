using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Net.Http;

namespace WebApplication1
{
    public class Startup
    {
        private Timer KeepAliveTimer;
        private HttpClient client = new HttpClient();

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            KeepAliveTimer = new Timer(x => CallAPI(), null, 0, 5000);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            else
            {
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            appLifetime.ApplicationStopping.Register(() => {
                KeepAliveTimer.Dispose();
            });

            appLifetime.ApplicationStopped.Register(() => {
                KeepAliveTimer.Dispose();
            });
        }


        private void CallAPI()
        {

            try
            {
                string dummy = client.GetStringAsync(new Uri("http://www.samwheat.com/api/api/blog/GetContentItemBySlug?slug=Hello-World&siteID=12")).Result;
            }
            catch (Exception)
            {
                // dont care
            }
        }
    }
}
