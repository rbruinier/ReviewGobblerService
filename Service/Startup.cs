using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewGobbler.Shared.DAL;
using Microsoft.EntityFrameworkCore;
using ReviewGobbler.Service.Business.Players.AppleMusic;
using ReviewGobbler.Service.Business.Players.Spotify;

namespace ReviewGobbler.Service
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            _env = env;

            Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddJsonFile($"appsettings.{_env.EnvironmentName}.json")
              .AddEnvironmentVariables()
              .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine(Configuration);

            services.AddControllers();

            Console.WriteLine("Using DB at: " + Configuration.GetConnectionString("Default"));

            services
                .AddSingleton<ISpotifyPlayerResolver>(new SpotifyPlayerResolver())
                .AddSingleton<IAppleMusicPlayerResolver>(new AppleMusicPlayerResolver())
                .AddDbContext<DatabaseContext>(options => options.UseSqlite(Configuration.GetConnectionString("Default")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
