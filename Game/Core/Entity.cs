using Game.Application.SharedKernel;
using Newtonsoft.Json;

namespace Game.Core;

public abstract class Entity
{
    [JsonIgnore]
    public List<INotification> DomainEvents { get; private set; } = [];
    
    public void AddDomainEvent(INotification eventItem) => DomainEvents.Add(eventItem);
    
    public void RemoveDomainEvent(INotification eventItem) => DomainEvents?.Remove(eventItem);
    public void ResetDomainEvents() => DomainEvents = [];
}
