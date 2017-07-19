using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Neutrino.Core.Services.Parameters;
using Neutrino.Entities;

namespace Neutrino.Core.Infrastructure
{
    public class StoreContext : IStoreContext
    {
        public SqliteConnection Repository { get; private set; }

        private readonly ApplicationParameters _applicationParameters;

        public StoreContext(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters.Value;
            Repository = new SqliteConnection(_applicationParameters.ConnectionStrings.DefaultConnection);
            Repository.Open();

            CreateTableIfNotExists();
        }

        private bool disposedValue = false;

        private void CreateTableIfNotExists()
        {
            string table = "CREATE TABLE IF NOT EXISTS nosql (type varchar(400), id varchar(100), json text)";
            using(var command = new SqliteCommand(table, Repository))
            {
                command.ExecuteNonQuery();
            }

            string index = "CREATE UNIQUE INDEX IF NOT EXISTS nosql_primary_key ON nosql (type, id)";
            using(var command = new SqliteCommand(index, Repository))
            {
                command.ExecuteNonQuery();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Repository.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}