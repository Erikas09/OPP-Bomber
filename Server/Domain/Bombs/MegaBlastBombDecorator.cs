namespace Bomber.Domain.Bombs;

/// <summary>
/// Concrete Decorator (Layer 3) — doubles the bomb's explosion radius,
/// making it a "mega blast" bomb.
/// Executes its behavior BEFORE delegating to the parent for the radius.
/// </summary>
public class MegaBlastBombDecorator : BombDecorator
{
    private readonly int _radiusMultiplier;

    public MegaBlastBombDecorator(IBomb wrappee)
        : this(wrappee, multiplier: 2)
    {
    }

    public MegaBlastBombDecorator(IBomb wrappee, int multiplier)
        : base(wrappee)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(multiplier, 1);
        _radiusMultiplier = multiplier;
    }

    /// <summary>
    /// Returns the wrapped bomb's radius multiplied by the mega blast factor.
    /// If multiple MegaBlastBombDecorators are stacked, they compound.
    /// </summary>
    public override int Radius => base.Radius * _radiusMultiplier;

    public override string GetDescription()
    {
        return base.GetDescription() + $" + Mega Blast (x{_radiusMultiplier})";
    }
}
