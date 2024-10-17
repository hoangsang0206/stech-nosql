using MongoDB.Driver;

namespace STech.Data.Models;

public partial class StechDbContext
{
    private readonly IMongoDatabase _database;

    public StechDbContext(string connectionString, string databaseName) {
        MongoClient client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName) {
        return _database.GetCollection<T>(collectionName);
    }
}
