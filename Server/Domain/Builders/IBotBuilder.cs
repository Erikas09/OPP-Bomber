using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;

namespace Bomber.Domain.Builder;

public interface IBotBuilder
{
    IBotBuilder Reset();
    IBotBuilder SetPosition(Position position);
    IBotBuilder SetLives(int lives);
    IBotBuilder SetBombLimit(int bombLimit);
    IBotBuilder SetExplosionRadius(int explosionRadius);
    IBotBuilder SetSpeed(float speed);
    IBotBuilder SetStrategy(BotStrategy strategy);
    Bot Build();

}
