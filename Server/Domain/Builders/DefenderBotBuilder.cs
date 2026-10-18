using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;

namespace Bomber.Domain.Builders;

internal class DefenderBotBuilder : IBotBuilder
{
    private Position position;
    private int lives;
    private int bombLimit;
    private int explosionRadius;
    private float speed;
    private BotStrategy strategy = null!;

    public DefenderBotBuilder() => Reset();

    public IBotBuilder Reset()
    {
        position = new(0, 0);
        lives = 0;
        bombLimit = 0;
        explosionRadius = 0;
        speed = 0;
        strategy = null!;
        return this;
    }

    public IBotBuilder SetPosition(Position position)
    {
        this.position = position;
        return this;
    }

    public IBotBuilder SetLives(int lives)
    {
        this.lives = lives;
        return this;
    }

    public IBotBuilder SetBombLimit(int bombLimit)
    {
        this.bombLimit = bombLimit;
        return this;
    }

    public IBotBuilder SetExplosionRadius(int radius)
    {
        explosionRadius = radius;
        return this;
    }

    public IBotBuilder SetSpeed(float speed)
    {
        this.speed = speed;
        return this;
    }

    public IBotBuilder SetStrategy(BotStrategy strategy)
    {
        this.strategy = strategy;
        return this;
    }

    public Bot Build()
    {
        Bot bot = new Bot(
            Guid.NewGuid(),
            "Hunter",
            position,
            lives,
            bombLimit,
            explosionRadius,
            speed
        );

        bot.SetStrategy(strategy);

        Reset();

        return bot;
    }
}
