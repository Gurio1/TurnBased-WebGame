using StackExchange.Redis;

namespace Game.Features.Battle;

public class RedisConnectionFactory
{
    public ConnectionMultiplexer CreateConnection()
    {
        return ConnectionMultiplexer.Connect("localhost:6379");
    }
}