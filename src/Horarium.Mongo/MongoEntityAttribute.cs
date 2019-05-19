using System;

namespace Horarium.Mongo
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