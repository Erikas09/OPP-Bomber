using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;

namespace Bomber.Domain.Builder;

public interface IBotBuilder
{
    void Reset();
    void SetPosition(Position position);
    void SetLives(int lives);
    void SetBombLimit(int bombLimit);
    void SetExplosionRadius(int explosionRadius);
    void SetSpeed(float speed);
    void SetStrategy(BotStrategy strategy);
    Bot Build();

}
