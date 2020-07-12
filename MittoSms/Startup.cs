using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using MittoSms.ServiceInterface;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Validation;
using ServiceStack.Text;
using System;

namespace MittoSms
{
    public class Startup : ModularStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("MittoSms", typeof(SMSServices).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                DefaultRedirectPath = "/metadata",
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
            });

            // global DateTime JSON configuration

            /*
            JsConfig<DateTime>.SerializeFn = time => new DateTime(time.Ticks, DateTimeKind.Local)
                .ToString("yyyy-MM-ddTHH:mm:ss");

            JsConfig<DateTime?>.SerializeFn =
                time => time != null
                    ? new DateTime(time.Value.Ticks, DateTimeKind.Local)
                        .ToString("yyyy-MM-ddTHH:mm:ss")
                    : null;
           */

            JsConfig.DateHandler = DateHandler.ISO8601;

            Plugins.Add(new ValidationFeature());
            container.RegisterValidators(typeof(SMSServices).Assembly);
            container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory("server=localhost;database=mitto;uid=mitto;pwd=mitto;", MySqlDialect.Provider));
            //TODO: init DB if tables do not exist
        }
    }
}