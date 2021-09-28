using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ASNA.QSys.Expo.Model;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace CustomerAppSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        void ConfigureMonaLisa(IServiceCollection services)
        {
            MonaServerConfig monaServerConfig = new MonaServerConfig();
            Configuration.GetSection("MonaServer").Bind(monaServerConfig);

            if (string.Compare(monaServerConfig.HostName, "*InProcess", true) == 0)
            {
                ASNA.QSys.MonaServer.Server.StartService("*LoopBack", monaServerConfig.Port);
            }

            services.AddSingleton<IMonaServerConfig>(s => monaServerConfig);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // AJAX will need to read Request.Body to be read synchronously  
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.ConfigureDisplayPagesOptions(Configuration);
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.IsEssential = true; // make the session cookie Essential -- Problematic for DGPR??
            });

            // Session data needs a store. AddMemoryCache activates a local memory-based store. 
            services.AddMemoryCache();

            services.AddRazorPages(razorOptions =>
            {
                razorOptions.Conventions.AddAreaPageRoute("CustomerAppViews", "/CUSTDELIV", "");
            }).AddMvcOptions(mvcOptions =>
            {
                mvcOptions.ValueProviderFactories.Insert(0, new EditedValueProviderFactory());
            });

            ConfigureMonaLisa(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }
    }
}
