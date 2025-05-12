using StackExchange.Redis;

namespace Game.Persistence.Redis;

public class RedisProvider
{
    private readonly Lazy<ConnectionMultiplexer> connection;
    
    public RedisProvider(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("RedisConnection");
        
        connection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(connectionString));
    }
    
    public ConnectionMultiplexer GetConnection() => connection.Value;
    
    public IDatabase GetDatabase() => connection.Value.GetDatabase();
}
