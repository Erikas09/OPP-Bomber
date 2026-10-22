namespace Bomber.Domain.Bombs;

/// <summary>
/// Concrete Decorator (Layer 1) — makes the bomb's explosion pierce through
/// destructible walls instead of stopping at them.
/// Executes its behavior AFTER delegating to the parent.
/// </summary>
public class PiercingBombDecorator : BombDecorator
{
    public PiercingBombDecorator(IBomb wrappee) : base(wrappee)
    {
    }

    /// <summary>
    /// Overrides the piercing flag — this bomb's explosion goes through
    /// destructible walls and continues propagating.
    /// </summary>
    public override bool IsPiercing => true;

    public override string GetDescription()
    {
        return base.GetDescription() + " + Piercing";
    }
}
