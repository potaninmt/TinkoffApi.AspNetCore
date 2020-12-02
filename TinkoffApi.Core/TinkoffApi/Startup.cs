using AutoTrading.BL.Services;
using AutoTrading.BL.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TinkoffApi.BL.Hubs;
using TinkoffApi.DAL;

namespace AutoTrading
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Scoped
            services.AddScoped<ITinkoffService, TinkoffService>();


            //Singleton
            services.AddSingleton<IConfiguration>(this.Configuration);

            //Hosted
            services.AddSingleton<TinkoffApi.BL.Hosted.BackgroundService>();
            services.AddSingleton<IHostedService, TinkoffApi.BL.Hosted.BackgroundService>(
                serviceProvider => serviceProvider.GetService<TinkoffApi.BL.Hosted.BackgroundService>());
            //services.AddSingleton<IHostedService, TinkoffApi.BL.Hosted.BackgroundService>();
            //services.AddHostedService<TinkoffApi.BL.Hosted.BackgroundService>();

            //SignalR
            services.AddSignalR();

            var connectionString = this.Configuration.GetConnectionString("PgConnectionsString");
            services.AddDbContextPool<TinkoffApiContext>(options =>
            {
                options.UseNpgsql(connectionString, config => config.MigrationsAssembly("TinkoffApi"));
                options.EnableSensitiveDataLogging(true);
                options.EnableDetailedErrors(true);
            });

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "TinkoffApi",
                    Description = "Тестирование методов в контроллерах",
                    Version = "v1"
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            // Migration
            InitializeDatabase(app);
            //tinkoffApiContext.LogContexts.ToArray();

            var tinkoffService = serviceProvider.GetRequiredService<ITinkoffService>();
            tinkoffService.SaveCurrentPortfolioBDAsync();


            // swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TinkoffApi v1");
            });


            app.UseSignalR(configure =>
            {
                configure.MapHub<TinkoffHub>("/tinkoffHub");

            });

            app.UseCors(config => config.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseMvc(routes =>
            {
                //routes.MapRoute("default", "{controller=Home}/{action=FileView}/{id?}");
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<TinkoffApiContext>().Database.Migrate();
            }
        }

        private static string GetXmlCommentsPath()
        {
            return String.Format(@"{0}\TinkoffApi.xml", AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
