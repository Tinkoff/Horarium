using System;
using System.Collections.Concurrent;
using System.Reflection;
using Horarium.Repository;
using MongoDB.Driver;

namespace Horarium.Mongo
{
    public sealed class MongoClientProvider : IMongoClientProvider
    {
        private readonly ConcurrentDictionary<Type, string> _collectionNameCache = new ConcurrentDictionary<Type, string>();

        private readonly Lazy<MongoClient> _mongoClient;
        private readonly string _databaseName;

        public MongoClientProvider(string mongoConnectionString)
        {
            var mongoUrl = new MongoUrl(mongoConnectionString);
            _databaseName = mongoUrl.DatabaseName;
            _mongoClient = new Lazy<MongoClient>(() => new MongoClient(mongoUrl));
            CreateIndexes();
        }

        private string GetCollectionName(Type entityType)
        {
            var collectionAttr = entityType.GetTypeInfo().GetCustomAttribute<MongoEntityAttribute>();

            if (collectionAttr == null)
                throw new InvalidOperationException($"Entity with type '{entityType.GetTypeInfo().FullName}' is not Mongo entity (use MongoEntityAttribute)");

            return collectionAttr.CollectionName;
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            var collectionName = _collectionNameCache.GetOrAdd(typeof(TEntity), GetCollectionName);
            return _mongoClient.Value.GetDatabase(_databaseName).GetCollection<TEntity>(collectionName);
        }

        private void CreateIndexes()
        {
            var issueCollection = GetCollection<JobMongoModel>();

            issueCollection.Indexes.CreateOne(Builders<JobMongoModel>.IndexKeys
                .Ascending(x => x.Status)
                .Ascending(x=>x.StartAt)
                .Ascending(x=>x.StartedExecuting),
                new CreateIndexOptions
                {
                    Background = true
                });

            issueCollection.Indexes.CreateOne(Builders<JobMongoModel>.IndexKeys
                    .Ascending(x => x.Status)
                    .Ascending(x => x.JobKey),
                new CreateIndexOptions
                {
                    Background = true
                });

            issueCollection.Indexes.CreateOne(Builders<JobMongoModel>.IndexKeys
                    .Ascending(x => x.JobKey),
                new CreateIndexOptions
                {
                    Background = true
                });
        }
    }
}