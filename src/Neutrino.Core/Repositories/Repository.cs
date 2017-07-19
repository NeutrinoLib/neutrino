using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Sqlite;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;
using Newtonsoft.Json;

namespace Neutrino.Core.Repositories
{
    public class Repository<T> : IRepository<T> where T: BaseEntity
    {
        private readonly IStoreContext _storeContext;
        
        public Repository(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public IEnumerable<T> Get()
        {
            var collection = new List<T>();

            string sql = "SELECT json FROM nosql WHERE type = @type";
            using (SqliteCommand command = new SqliteCommand(sql, _storeContext.Repository))
            {
                var collectionType = GetCollectionType();
                command.Parameters.Add(new SqliteParameter("type", collectionType));

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        string json = reader.GetString(0);
                        var jsonObject = JsonConvert.DeserializeObject<T>(json);
                        collection.Add(jsonObject);
                    }         
                }
            }

            return collection;
        }

        public IEnumerable<T> Get(Func<T, bool> predicate)
        {
            var collection = Get();
            var query = collection.Where(predicate);
            return query;
        }

        public T Get(string id)
        {
            string sql = "SELECT json FROM nosql WHERE type = @type AND id = @id";
            using (SqliteCommand command = new SqliteCommand(sql, _storeContext.Repository))
            {
                var collectionType = GetCollectionType();
                command.Parameters.Add(new SqliteParameter("type", collectionType));
                command.Parameters.Add(new SqliteParameter("id", id));

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) 
                    {
                        string json = reader.GetString(0);
                        var jsonObject = JsonConvert.DeserializeObject<T>(json);
                        return jsonObject;
                    }         
                }
            }

            return null;
        }

        public void Create(T entity)
        {
            string sql = "INSERT INTO nosql (type, id, json) VALUES (@type, @id, @json)";
            using (SqliteCommand command = new SqliteCommand(sql, _storeContext.Repository))
            {
                var collectionType = GetCollectionType();
                var json = JsonConvert.SerializeObject(entity);

                command.Parameters.Add(new SqliteParameter("type", collectionType));
                command.Parameters.Add(new SqliteParameter("id", entity.Id));
                command.Parameters.Add(new SqliteParameter("json", json));

                command.ExecuteNonQuery();
            }
        }

        public void Update(T entity)
        {
            string sql = "UPDATE nosql SET json = @json WHERE type = @type AND id = @id";
            using (SqliteCommand command = new SqliteCommand(sql, _storeContext.Repository))
            {
                var collectionType = GetCollectionType();
                var json = JsonConvert.SerializeObject(entity);

                command.Parameters.Add(new SqliteParameter("type", collectionType));
                command.Parameters.Add(new SqliteParameter("id", entity.Id));
                command.Parameters.Add(new SqliteParameter("json", json));

                command.ExecuteNonQuery();
            }
        }

        public void Delete(string id)
        {
            string sql = "DELETE FROM nosql WHERE type = @type AND id = @id";
            using (SqliteCommand command = new SqliteCommand(sql, _storeContext.Repository))
            {
                var collectionType = GetCollectionType();

                command.Parameters.Add(new SqliteParameter("type", collectionType));
                command.Parameters.Add(new SqliteParameter("id", id));

                command.ExecuteNonQuery();
            }
        }

        public void Clear()
        {
            string sql = "DELETE FROM nosql WHERE type = @type ";
            using (SqliteCommand command = new SqliteCommand(sql, _storeContext.Repository))
            {
                var collectionType = GetCollectionType();
                command.Parameters.Add(new SqliteParameter("type", collectionType));

                command.ExecuteNonQuery();
            }
        }

        private string GetCollectionType()
        {
            var type = typeof(T).Name;
            return type;
        }
    }
}