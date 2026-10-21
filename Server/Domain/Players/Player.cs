using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.PowerUps;

namespace Bomber.Domain.Players;

public class Player
{
    public const float PlayerSize = 30f;

    public Guid Id { get; }

    public string Name { get; private set; }

    public Position Position { get; private set; }

    public int Lives { get; private set; }

    public int BombLimit { get; private set; }

    public int ExplosionRadius { get; private set; }

    public float Speed { get; private set; }

    public PlayerState State { get; private set; }
    public PlayerInput Input { get; private set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }

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
    public void TakeDamage()
    {
        if (State == PlayerState.Dead)
            return;

        Lives--;

        if (Lives <= 0)
        {
            Lives = 0;
            State = PlayerState.Dead;
        }
    }

    public void SetSpeed(float speed)
    {
        this.Speed = speed;
    }
    public float GetSpeed()
    {
        return this.Speed;
    }
    public void SetExplosionRadius(int radius)
    {
        this.ExplosionRadius = radius;
    }
    public int GetExplosionRadius()
    {
        return this.ExplosionRadius;
    }
    public void SetBombLimit(int limit)
    {
        this.BombLimit = limit;
    }
    public int GetBombLimit()
    {
        return this.BombLimit;
    }
    public void PickUpPowerUp(PowerUp powerUp)
    {
        powerUp.ApplyEffect(this);       
    }
}
