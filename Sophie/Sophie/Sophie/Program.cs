using System;
using System.Reflection;
using System.Security.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using App.Security;
using Newtonsoft.Json.Linq;
using App.Core.Units;
using MongoDB.Driver;
using Sophie.Units;

namespace Sophie
{
    public class Program
    {
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(Program));

        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //}

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        [Obsolete]
        public static void Main(string[] args)
        {
            //EmailSender.TestExecuteSendEmail();

            // Load configuration log4net
            var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            #if DEBUG
            log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.Development.config"));
            #else
            log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));
            #endif

            // Get enviroment from envsettings.json
            var currentDirectoryPath = System.IO.Directory.GetCurrentDirectory();
            var envSettingsPath = System.IO.Path.Combine(currentDirectoryPath, "envsettings.json");
            var envSettings = JObject.Parse(System.IO.File.ReadAllText(envSettingsPath));
            var enviromentValue = envSettings.Value<string?>("ASPNETCORE_ENVIRONMENT") ?? "Production";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{enviromentValue}.json", optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .Build();

            if (CheckDbConnection(configuration))
            {
                bool startBrowser = false;
                #if DEBUG
                startBrowser = true;
                #endif
                IHost server = ProgramExtensions.CreateServer<Startup>(configuration, startBrowser, args);
                if (server == null) return;
                server.Run();
            }
        }

        private static bool CheckDbConnection(IConfigurationRoot _configuration)
        {
            try
            {
                // mongodb://admin:26689#vt@127.0.0.1:27017
                // mongodb://sophie_user:Abc#1234@127.0.0.1:27017/sophie_db
                string _connectionString = _configuration["MongoSettings:ConnectionString"];
                string _host = _configuration["MongoSettings:Host"];
                string _port = _configuration["MongoSettings:Port"];
                string _user = _configuration["MongoSettings:User"];
                string _pass = _configuration["MongoSettings:Pass"];
                string _db = _configuration["MongoSettings:Db"];
                if (!string.IsNullOrEmpty(_host) && !string.IsNullOrEmpty(_host) && !string.IsNullOrEmpty(_user) && !string.IsNullOrEmpty(_pass) && !string.IsNullOrEmpty(_db))
                    _connectionString = $"mongodb://{_user}:{_pass}@{_host}:{_port}/{_db}";

                _log4net.Info($"[Program] Connect: {_connectionString}, Database: {_db}");
                MongoClient _client = new MongoClient($"{_connectionString}");//mongodb://admin:26689#vt@127.0.0.1:27017

                return true;
            }
            catch (Exception ex)
            {
                Logs.debug("Error: connection Database" + ex.StackTrace);
                _log4net.Error("Error: connection Database", ex);
                return false;
            }
        }

        // Uses Generic Host in .NET Core 3.x - Not change
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.ConfigureHttpsDefaults(listenOptions =>
                        {
                            listenOptions.SslProtocols = SslProtocols.Tls13;
                        });
                    });
                    webBuilder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                });
            return host;
        }

        // Uses Generic Host in .NET Core 2.x - Not change
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.ConfigureHttpsDefaults(listenOptions =>
                    {
                        listenOptions.SslProtocols = SslProtocols.Tls13;
                    });
                })
                .UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();
            return webHost;
        }

    }

}
