using System;
using System.Collections.Generic;
using Core.Utilities;

namespace Domain.Core.Services
{
    //todo 暂时放在这，后续移到基础层
    public class EventBus : Singleton<EventBus>
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        private readonly Dictionary<Type, List<Action<IDomainEvent>>> _handlers = new Dictionary<Type, List<Action<IDomainEvent>>>();
        
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
            PublishEvent(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<Action<IDomainEvent>>();
            }

            _handlers[eventType].Add((e) => handler((TEvent)e));
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].RemoveAll(h => h.Target == handler.Target && h.Method == handler.Method);
            }
        }

        private void PublishEvent(IDomainEvent domainEvent)
        {
            var eventType = domainEvent.GetType();
            if (_handlers.ContainsKey(eventType))
            {
                foreach (var handler in _handlers[eventType])
                {
                    handler(domainEvent);
                }
            }
        }
    }
    
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}