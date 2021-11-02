using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Scraping;

namespace ReviewGobbler.PitchforkScraper
{
    public class ConsoleApplication
    {
        private DatabaseContext _databaseContext;

        public ConsoleApplication(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task Run(string[] args)
        {
            Console.WriteLine("Pitchfork scraper booting up");

            var processor = new ReviewProcessor(1, new Scraper(), _databaseContext);

            await processor.ProcessReviews();
        }
    }

    class Program
    {
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

#if DEBUG
            var environmentName = "Development";
#else
            var environmentName = "Production";
#endif

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{environmentName}.json")
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton(configuration);

            services.AddSingleton<DatabaseContext, DatabaseContext>();

            services.AddTransient<ConsoleApplication>();

            services
                .AddEntityFrameworkSqlite()
                .AddDbContext<DatabaseContext>(options => options.UseSqlite(configuration.GetConnectionString("Default")));

            services.AddHttpClient();

            return services;
        }

        static async Task Main(string[] args)
        {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetService<ConsoleApplication>().Run(args);
        }
    }
}
