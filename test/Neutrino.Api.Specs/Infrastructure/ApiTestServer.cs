using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

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

        private static TestServer CreateTestServer()
        {
            RemoveLiteDbFile();

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>();

            return new TestServer(webHostBuilder);
        }

        private static void RemoveLiteDbFile()
        {
            File.Delete("nosqlstore.db");
            File.Delete("nosqlstore-journal.db");
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