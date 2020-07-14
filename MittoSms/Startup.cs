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
using MittoSms.Logic;
using ServiceStack.Logging.NLogger;
using ServiceStack.Logging;
using MittoSms.ServiceModel.Types;

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
            LogManager.LogFactory = new ConsoleLogFactory();

            InitJsConfig();
            InitDbConnection(container);
            InitValidations(container);

            container.Register<ICountryLookup>(c => new SimpleCountryLookup(4));
            container.Register<ISmsSender>(c => new RandomLogSmsSender());
        }

        private static void InitJsConfig()
        {
            JsConfig<DateTime>.SerializeFn = time =>
            {
                switch (JsConfig.DateHandler)
                {
                    case DateHandler.ISO8601:
                        return new DateTime(time.Ticks, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ss");
                    case DateHandler.ISO8601DateOnly:
                        return new DateTime(time.Ticks, DateTimeKind.Utc).ToString("yyyy-MM-dd");
                }
                return time.ToString();
            };
            JsConfig.DateHandler = DateHandler.ISO8601;
        }

        private void InitDbConnection(Container container)
        {
            var dbConnectionString = Configuration.GetValue<String>("Mitto:Db:Connection") ?? "server=localhost;database=mitto;uid=mitto;pwd=mitto;";
            var dbFactory = new OrmLiteConnectionFactory(dbConnectionString, MySqlDialect.Provider);
            container.Register<IDbConnectionFactory>(c => dbFactory);
            CheckAndCreateDb(dbFactory);
        }

        private static void CheckAndCreateDb(OrmLiteConnectionFactory dbFactory)
        {
            using (var db = dbFactory.Open())
            {
                if (db.CreateTableIfNotExists<Country>())
                {
                    db.Insert(new Country { Name = "Germany", Mcc = "262", Cc = "49", PricePerSMS = 0.55m });
                    db.Insert(new Country { Name = "Austria", Mcc = "232", Cc = "43", PricePerSMS = 0.53m });
                    db.Insert(new Country { Name = "Germany", Mcc = "260", Cc = "48", PricePerSMS = 0.32m });
                }
                db.CreateTableIfNotExists<Sms>();
            }
        }

        private void InitValidations(Container container)
        {
            Plugins.Add(new ValidationFeature());
            container.RegisterValidators(typeof(SMSServices).Assembly);
        }
    }
}