using System;
using System.Collections.Generic;
using ReusableScripts.Interface;
using UnityEngine;

namespace ReusableScripts.Core
{
    public class EventBus : Singleton<EventBus>
    {
        private Dictionary<Type, List<IEventHandler>> _eventHandlers = new Dictionary<Type, List<IEventHandler>>();
        
        private Dictionary<Type, int> _eventCounts = new Dictionary<Type, int>();

        [Header("Debug")] [SerializeField] private bool _logEvents = false;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Subscribe to an event type
        /// </summary>
        /// <typeparam name="T">The event type</typeparam>
        /// <param name="handler">The action to call when event is published</param>
        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            Type eventType = typeof(T);

            if (!_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType]  = new List<IEventHandler>();
            }

            var eventHandler = new EventHandler<T>(handler);
            _eventHandlers[eventType].Add(eventHandler);
            
            if(_logEvents)
                Debug.Log($"[EventBus] Subscribing to {eventType}]");
            
        }

        /// <summary>
        /// Unsubscribe from an event type
        /// </summary>
        /// <typeparam name="T">The event type</typeparam>
        /// <param name="handler">The action to remove</param>
        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            Type eventType = typeof(T);

            if (!_eventHandlers.ContainsKey(eventType)) return;

            var handlers = _eventHandlers[eventType];
            
            for (var i = 0; i < handlers.Count; i++)
            {
                if (handlers[i] is EventHandler<T> eventHandler && eventHandler.IsSameHandler(handler))
                {
                    handlers.RemoveAt(i);

                    if (_logEvents)
                    {
                        Debug.Log($"[EventBus] Unsubscribing to {eventType}]");
                    }

                    break;
                }
            }

            if (handlers.Count == 0)
            {
                _eventHandlers.Remove(eventType);
            }
        }

        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        /// <typeparam name="T">The event type</typeparam>
        /// <param name="eventData">The event data</param>
        public void Publish<T>(T eventData) where T : IGameEvent
        {
            Type eventType = typeof(T);

            if (!_eventCounts.ContainsKey(eventType))
            {
                _eventCounts[eventType] = 0;
            }

            if (_logEvents)
            {
                Debug.Log($"[EventBus] Publishing to {eventType.Name} | Count: {_eventCounts[eventType]}");
            }

            if (!_eventHandlers.ContainsKey(eventType))
            {
                if (_logEvents)
                {
                    Debug.Log($"[EventBus] No subscribers for {eventType.Name}]");
                }

                return;
            }

            // Invoke all handlers (iterate backwards to allow removal during iteration)
            var handlers = _eventHandlers[eventType];
            for (var i = handlers.Count - 1; i >= 0 ; i--)
            {
                try
                {
                    handlers[i].Handle(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventBus] Error handling {eventType.Name}: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Clear all event subscriptions (useful for cleanup)
        /// </summary>
        public void ClearAll()
        {
            _eventHandlers.Clear();
            _eventCounts.Clear();
            Debug.Log($"[EventBus] All subscription cleared");
        }

        /// <summary>
        /// Get subscription count for an event type (for debugging)
        /// </summary>
        public int GetSubscriberCount<T>() where T : IGameEvent
        {
            Type eventType = typeof(T);
            return _eventCounts.ContainsKey(eventType) ? _eventCounts[eventType] : 0;
        }

        protected override void OnDestroy()
        {
            ClearAll();
            base.OnDestroy();
        }
    }

    /// <summary>
    /// Generic event handler wrapper
    /// </summary>
    internal class EventHandler<T> : IEventHandler where T : IGameEvent
    {
        private readonly Action<T> _handler;

        public EventHandler(Action<T> handler)
        {
            _handler = handler;
        }
        
        public void Handle(IGameEvent gameEvent)
        {
            if (gameEvent is T typeEvent)
            {
                _handler.Invoke(typeEvent);
            }
        }

        public bool IsSameHandler(Action<T> otherHandler)
        {
            return _handler == otherHandler;
        }
    } 
}