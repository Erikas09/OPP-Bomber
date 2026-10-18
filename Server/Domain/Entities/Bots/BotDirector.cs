using Bomber.Domain.Builders;
using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;

namespace Bomber.Domain.Entities.Bots;

public class BotDirector
{
    public Bot ConstructHunter(IBotBuilder builder, Position position)
    {
        return builder
            .Reset()
            .SetPosition(position)
            .SetLives(3)
            .SetBombLimit(2)
            .SetExplosionRadius(3)
            .SetSpeed(120f)
            .SetStrategy(new HuntStrategy())
            .Build();
    }

    public Bot ConstructDefender(IBotBuilder builder, Position position)
    {
        return builder
            .Reset()
            .SetPosition(position)
            .SetLives(5)
            .SetBombLimit(1)
            .SetExplosionRadius(2)
            .SetSpeed(90f)
            .SetStrategy(new ExploreStrategy())
            .Build();
    }
}
