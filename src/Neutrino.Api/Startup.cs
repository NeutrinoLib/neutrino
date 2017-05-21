using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace Neutrino.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMemoryCache();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Neutrino API",
                    Description = "Neutrino is minimalistic service discovery application.",
                    TermsOfService = "None"
                });
            });

            services.AddSingleton<HttpClient, HttpClient>();
            services.AddSingleton<IStoreContext, StoreContext>();
            services.AddSingleton<IHealthService, HealthService>();

            services.AddScoped<INodesService, NodesService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IServiceHealthService, ServiceHealthService>();
            services.AddScoped<INodeHealthService, NodeHealthService>();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IServicesService servicesService)
        {
            if(env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                loggerFactory.AddAzureWebAppDiagnostics();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });

            servicesService.RunHealthChecker();
        }
    }
}
