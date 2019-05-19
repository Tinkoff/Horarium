using MongoDB.Driver;

namespace Horarium.Mongo
{
    public interface IMongoClientProvider
    {
        IMongoCollection<TEntity> GetCollection<TEntity>();
    }
}