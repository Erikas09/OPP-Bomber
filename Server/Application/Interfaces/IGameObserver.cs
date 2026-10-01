using Bomber.Domain.Events;

namespace Bomber.Application.Interfaces;

public interface IGameObserver
{
    void Update(GameEvent gameEvent);
}
