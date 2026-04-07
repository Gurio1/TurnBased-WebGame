using StackExchange.Redis;

namespace Game.Battle.Persistence.Cache.Redis;

public class RedisProvider
{
    private readonly Lazy<ConnectionMultiplexer> connection;
    
    public RedisProvider(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException();
        
        connection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(connectionString));
    }
    
    public ConnectionMultiplexer GetConnection() => connection.Value;
    
    public IDatabase GetDatabase() => connection.Value.GetDatabase();
}
