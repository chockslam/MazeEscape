using System.Collections.Generic;
using System;

public class Observer<T>
{
    private T _lastEvent;
    private readonly List<Action<T>> _subscribers = new();

    public T LastEvent
    {
        get { return _lastEvent; }
        protected set { _lastEvent = value; }
    }

    public void Subscribe(Action<T> listener)
    {
        if (!_subscribers.Contains(listener))
        {
            _subscribers.Add(listener);
        }
    }

    public void Unsubscribe(Action<T> listener)
    {
        _subscribers.Remove(listener);
    }

    public void NotifySubscribers(T newEvent)
    {
        _lastEvent = newEvent; // Update the current state

        foreach (var subscriber in _subscribers)
        {
            subscriber?.Invoke(newEvent);
        }
    }
}