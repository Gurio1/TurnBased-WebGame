using StackExchange.Redis;

namespace Game.Battle;

public class RedisConnectionFactory
{
    public ConnectionMultiplexer CreateConnection()
    {
        return ConnectionMultiplexer.Connect("localhost:6379");
    }
}