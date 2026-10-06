using Bomber.Domain.Builder;
using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;

public class HunterBotBuilder : IBotBuilder
{
    private Position position;
    private int lives;
    private int bombLimit;
    private int explosionRadius;
    private float speed;
    private BotStrategy strategy = null!;

    public HunterBotBuilder() => Reset();

    public void Reset()
    {
        position = new(0, 0);
        lives = 0;
        bombLimit = 0;
        explosionRadius = 0;
        speed = 0;
        strategy = null!;
    }

    public void SetPosition(Position position)
    {
        this.position = position;
    }

    public void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void SetBombLimit(int bombLimit)
    {
        this.bombLimit = bombLimit;
    }

    public void SetExplosionRadius(int radius)
    {
        explosionRadius = radius;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetStrategy(BotStrategy strategy)
    {
        this.strategy = strategy;
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