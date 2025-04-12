using StackExchange.Redis;

namespace Game.Features.Battle;

public class RedisProvider
{
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisProvider(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RedisConnection");
        _connection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(connectionString));
    }

    public ConnectionMultiplexer GetConnection() => _connection.Value;

    public IDatabase GetDatabase() => _connection.Value.GetDatabase();
}