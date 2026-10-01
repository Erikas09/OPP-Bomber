using Bomber.Application.Interfaces;
using Bomber.Domain.Events;

namespace Bomber.Application.Publishers;

public sealed class GameEventPublisher
{
    private readonly Lock _lock = new();

    private readonly List<IGameObserver> _observers = [];

    public void Subscribe(IGameObserver observer)
    {
        lock (_lock)
        {
            if (_observers.Contains(observer))
                return;

            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IGameObserver observer)
    {
        lock (_lock)
            _observers.Remove(observer);
    }

    public void Notify(GameEvent gameEvent)
    {
        List<IGameObserver> observers;

        lock (_lock)
            observers = [.. _observers];

        foreach (IGameObserver observer in observers)
            observer.Update(gameEvent);
    }
}