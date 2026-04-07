using System.Text.Json.Serialization;
using Game.SharedKernel.Domain;

namespace Game.SharedKernel.Models;

public abstract class Entity
{
    [JsonIgnore]
    public List<IDomainEvent> DomainEvents { get; private set; } = [];
    
    public void AddDomainEvent(IDomainEvent eventItem) => DomainEvents.Add(eventItem);
    
    public void RemoveDomainEvent(IDomainEvent eventItem) => DomainEvents?.Remove(eventItem);
    public void ResetDomainEvents() => DomainEvents = [];
}
