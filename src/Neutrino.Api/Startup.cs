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
using Microsoft.Extensions.Options;
using Neutrino.Consensus;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Options;
using Neutrino.Consensus.States;
using Neutrino.Core.Diagnostics;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Core.Services;
using Neutrino.Core.Services.Parameters;
using Neutrino.Core.Services.Validators;
using Neutrino.Entities;
using Newtonsoft.Json;
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
            services.Configure<ApplicationParameters>(Configuration);

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

            services.AddScoped<IRepository<Service>, Repository<Service>>();
            services.AddScoped<IRepository<ServiceHealth>, Repository<ServiceHealth>>();

            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IServiceHealthService, ServiceHealthService>();

            services.AddScoped<IServiceValidator, ServiceValidator>();

            services.AddConsensus<StateObserverService, LogReplicationService>();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IServicesService servicesService,
            IServiceHealthService serviceHealthService,
            IOptions<ApplicationParameters> applicationParameters)
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

            app.UseCustomExceptionHandler();

            app.UseConsensus(options => {
                options.CurrentNode = applicationParameters.Value.CurrentNode;
                options.Nodes = applicationParameters.Value.Nodes;
                options.MinElectionTimeout = applicationParameters.Value.MinElectionTimeout;
                options.MaxElectionTimeout = applicationParameters.Value.MaxElectionTimeout;
                options.HeartbeatTimeout = applicationParameters.Value.HeartbeatTimeout;
            });

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
        }
    }
}
