using MongoDB.Driver;

namespace Horarium.MongoRepository
{
    public interface IMongoClientProvider
    {
        IMongoCollection<TEntity> GetCollection<TEntity>();
    }
}