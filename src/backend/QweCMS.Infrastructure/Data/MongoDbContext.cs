using MongoDB.Driver;
using Microsoft.Extensions.Options;
using QweCMS.Core.Entities;
using QweCMS.Core.Settings;

namespace QweCMS.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        _database = client.GetDatabase(mongoSettings.Value.Database);
    }

    public IMongoDatabase Database => _database;

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

    // Коллекции для основных сущностей
    public IMongoCollection<MixinEntity> Mixins => GetCollection<MixinEntity>("mixins");
    public IMongoCollection<SchemaEntity> Schemas => GetCollection<SchemaEntity>("schemas");
    
    // Динамические коллекции для контента
    public IMongoCollection<dynamic> GetContentCollection(string collectionName)
    {
        return _database.GetCollection<dynamic>(collectionName);
    }
}