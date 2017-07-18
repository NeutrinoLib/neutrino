using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Neutrino.Api.SecureTokenAuthentication;
using Neutrino.Consensus;
using Neutrino.Core.Diagnostics;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Core.Services;
using Neutrino.Core.Services.Parameters;
using Neutrino.Core.Services.Validators;
using Neutrino.Entities;
using Swashbuckle.AspNetCore.Swagger;

namespace Neutrino.Api
{
    /// <summary>
    /// Web startup class.
    /// </summary>
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Constructor which initilizes configuration.
        /// </summary>
        /// <param name="hostingEnvironment">Environment variables.</param>
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        /// <summary>
        /// Configure dependency injection.
        /// </summary>
        /// <param name="services">Services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationParameters>(_configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(SecureTokenDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });

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

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPathApi = Path.Combine(basePath, "Neutrino.Api.xml"); 
                options.IncludeXmlComments(xmlPathApi);

                var xmlPathConsensus = Path.Combine(basePath, "Neutrino.Consensus.xml"); 
                options.IncludeXmlComments(xmlPathConsensus);
            });

            services.AddAuthorization();

            services.AddSingleton<HttpClient, HttpClient>();
            services.AddSingleton<IStoreContext, StoreContext>();
            services.AddSingleton<IHealthService, HealthService>();

            services.AddScoped<IRepository<Service>, Repository<Service>>();
            services.AddScoped<IRepository<ServiceHealth>, Repository<ServiceHealth>>();
            services.AddScoped<IRepository<KvProperty>, Repository<KvProperty>>();

            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IServiceHealthService, ServiceHealthService>();
            services.AddScoped<IKvPropertyService, KvPropertyService>();

            services.AddScoped<IServiceValidator, ServiceValidator>();
            services.AddScoped<IKvPropertyValidator, KvPropertyValidator>();

            services.AddConsensus<StateObserverService, LogReplicationService>();
        }

        /// <summary>
        /// Configure web application.
        /// </summary>
        /// <param name="applicationBuilder">Application builder.</param>
        /// <param name="hostingEnvironment">Environment variables.</param>
        /// <param name="loggerFactory">Logger.</param>
        /// <param name="applicationParameters">Application parameters.</param>
        public void Configure(
            IApplicationBuilder applicationBuilder, 
            IHostingEnvironment hostingEnvironment, 
            ILoggerFactory loggerFactory,
            IOptions<ApplicationParameters> applicationParameters)
        {
            if(hostingEnvironment.IsDevelopment())
            {
                loggerFactory.AddConsole(_configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                loggerFactory.AddAzureWebAppDiagnostics();
            }

            applicationBuilder.UseCustomExceptionHandler();
            applicationBuilder.UseCors("AllowAllOrigins");

            var secureTokenOptions = new SecureTokenOptions
            {
                SecureToken = _configuration["SecureToken"],
                AuthenticationScheme = SecureTokenDefaults.AuthenticationScheme,
                Realm = SecureTokenDefaults.Realm
            };
            applicationBuilder.UseSecureTokenAuthentication(secureTokenOptions);

            applicationBuilder.UseMvc();

            applicationBuilder.UseConsensus(options => {
                options.CurrentNode = applicationParameters.Value.CurrentNode;
                options.Nodes = applicationParameters.Value.Nodes;
                options.MinElectionTimeout = applicationParameters.Value.MinElectionTimeout;
                options.MaxElectionTimeout = applicationParameters.Value.MaxElectionTimeout;
                options.HeartbeatTimeout = applicationParameters.Value.HeartbeatTimeout;
                options.AuthenticationParameter = applicationParameters.Value.SecureToken;
                options.AuthenticationScheme = "SecureToken";
            });

            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
        }
    }
}
