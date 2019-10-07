using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GeoNames2Sql
{
    class Program
    {
        private static IConfiguration _configuration;
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            ConfigureConsole();
            RegisterServices();

            try
            {
                var ui = _serviceProvider.GetService<UIService>();
                await ui.Start();
            }
            finally
            {
                DisposeServices();
            }
        }

        private static void ConfigureConsole()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }

        private static void RegisterServices()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var services = ConfigureServices();
            _serviceProvider = services.BuildServiceProvider();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.Configure<AppSettings>(_configuration);
            services.AddOptions();
            services.AddTransient<AlternateNamesRepository>();
            services.AddTransient<CountryInfoRepository>();
            services.AddTransient<GeoNamesRepository>();
            services.AddTransient<GeoNamesService>();
            services.AddTransient<UIService>();

            return services;
        }

        private static void DisposeServices()
        {
            if (_serviceProvider is null)
                return;

            if (_serviceProvider is IDisposable provider)
                provider.Dispose();
        }
    }
}