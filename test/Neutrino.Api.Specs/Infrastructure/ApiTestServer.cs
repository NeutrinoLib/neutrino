using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Neutrino.Api;

namespace Neutrino.Api.Specs.Infrastructure
{
    public sealed class ApiTestServer : IDisposable
    {
        private static volatile TestServer _instance;
        private static object _syncRoot = new object();
        private bool _disposed = false;

        private ApiTestServer() { }

        public static TestServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = CreateTestServer();
                        }
                    }
                }

                return _instance;
            }
        }

        public static HttpClient GetHttpClient()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var secureToken = configuration["SecureToken"];

            var httpClient = Instance.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SecureToken", secureToken);

            return httpClient;
        }

        private static TestServer CreateTestServer()
        {
            RemoveDbFile();

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>();

            return new TestServer(webHostBuilder);
        }

        private static void RemoveDbFile()
        {
            File.Delete("database.sqlite");
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _instance.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ApiTestServer()
        {
            Dispose(false);
        }
    }
}