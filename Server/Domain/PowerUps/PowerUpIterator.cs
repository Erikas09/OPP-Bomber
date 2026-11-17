using Bomber.Domain.Common.Interfaces;

namespace Bomber.Domain.PowerUps;

public sealed class PowerUpIterator : IIterator<PowerUp>
{
    private readonly List<PowerUp> _powerUps;
    private int _index;

    public PowerUpIterator(List<PowerUp> powerUps)
    {
        _powerUps = powerUps;
    }

    public bool HasNext()
    {
        return _index < _powerUps.Count;
    }

    public PowerUp Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more power-ups.");

        PowerUp powerUp = _powerUps[_index];
        _index++;

        return powerUp;
    }

    public void Reset()
    {
        _index = 0;
    }
}
