using Bomber.Domain.Common;
using Bomber.Domain.Enums;

namespace Bomber.Domain.Players;

public class Player
{
    public Guid Id { get; }

    public string Name { get; private set; }

    public Position Position { get; private set; }

    public int Lives { get; private set; }

    public int BombLimit { get; private set; }

    public int ExplosionRadius { get; private set; }

    public float Speed { get; private set; }

    public PlayerState State { get; private set; }
    public PlayerInput Input { get; private set; }

    public Player(
        Guid id,
        string name,
        Position position,
        int lives,
        int bombLimit,
        int explosionRadius,
        float speed)
    {
        Id = id;
        Name = name;
        Position = position;

        Lives = lives;
        BombLimit = bombLimit;
        ExplosionRadius = explosionRadius;
        Speed = speed;

        State = PlayerState.Alive;
        Input = PlayerInput.None;
    }

    public void SetInput(PlayerInput input)
    {
        Input = input;
    }

    public void MoveTo(Position position)
    {
        if (State == PlayerState.Dead)
            return;

        Position = position;
    }
}
