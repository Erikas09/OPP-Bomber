namespace Bomber.Domain.Bombs;

/// <summary>
/// Concrete Decorator (Layer 2) — switches the bomb to remote detonation mode.
/// The bomb will never auto-explode via timer; ShouldExplode always returns false
/// until the player manually triggers it via Detonate().
/// Executes its behavior BEFORE delegating to the parent for the check.
/// </summary>
public class RemoteDetonationBombDecorator : BombDecorator
{
    private bool _detonated;

    public RemoteDetonationBombDecorator(IBomb wrappee) : base(wrappee)
    {
        _detonated = false;
    }

    public override bool IsRemoteDetonation => true;

    /// <summary>
    /// Overrides detonation logic — the bomb only explodes when
    /// manually detonated, not by the fuse timer.
    /// </summary>
    public override bool ShouldExplode(DateTimeOffset now)
    {
        // Remote detonation: ignore the timer, only explode when triggered
        if (_detonated)
            return true;

        return false;
    }

    /// <summary>
    /// Manually triggers the remote detonation.
    /// </summary>
    public void Detonate()
    {
        _detonated = true;
    }

    public override bool TryDetonate()
    {
        _detonated = true;
        return true;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + " + Remote Detonation";
    }
}
