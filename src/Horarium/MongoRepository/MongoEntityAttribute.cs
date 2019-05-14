using System;

namespace Horarium.MongoRepository
{
    public class MongoEntityAttribute: Attribute
    {
        public MongoEntityAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
        
        public string CollectionName { get; }
    }
}