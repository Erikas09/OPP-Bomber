using Bomber.Domain.Builder;
using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;

namespace Bomber.Domain.Entities.Bots;

public class BotDirector
{
    public void ConstructHunter(
        IBotBuilder builder,
        Position position)
    {
        builder.Reset();

        builder.SetPosition(position);
        builder.SetLives(3);
        builder.SetBombLimit(2);
        builder.SetExplosionRadius(3);
        builder.SetSpeed(120f);
        builder.SetStrategy(new HuntStrategy());
    }

    public void ConstructDefender(
        IBotBuilder builder,
        Position position)
    {
        builder.Reset();

        builder.SetPosition(position);
        builder.SetLives(5);
        builder.SetBombLimit(1);
        builder.SetExplosionRadius(2);
        builder.SetSpeed(90f);
        builder.SetStrategy(new ExploreStrategy());
    }
}
