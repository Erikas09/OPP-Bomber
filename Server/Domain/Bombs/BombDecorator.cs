using Bomber.Domain.Common;

namespace Bomber.Domain.Bombs;

/// <summary>
/// Base Decorator — stores a reference to a wrapped IBomb object and
/// delegates all operations to it. The field is declared with the IBomb
/// interface type so it can link to both concrete components and other decorators.
/// </summary>
public abstract class BombDecorator : IBomb
{
    /// <summary>
    /// Reference to the wrapped component (can be a Bomb or another decorator).
    /// </summary>
    protected readonly IBomb _wrappee;

    protected BombDecorator(IBomb wrappee)
    {
        _wrappee = wrappee ?? throw new ArgumentNullException(nameof(wrappee));
    }

    // ── All interface members delegate to the wrapped object ──

    public virtual Guid Id => _wrappee.Id;

    public virtual Guid OwnerId => _wrappee.OwnerId;

    public virtual GridPosition Position => _wrappee.Position;

    public virtual int Radius => _wrappee.Radius;

    public virtual DateTimeOffset PlacedAt => _wrappee.PlacedAt;

    public virtual int FuseSeconds => _wrappee.FuseSeconds;

    public virtual bool IsPiercing => _wrappee.IsPiercing;

    public virtual bool IsRemoteDetonation => _wrappee.IsRemoteDetonation;

    public virtual bool ShouldExplode(DateTimeOffset now)
    {
        return _wrappee.ShouldExplode(now);
    }

    public virtual string GetDescription()
    {
        return _wrappee.GetDescription();
    }

    public virtual bool TryDetonate()
    {
        return _wrappee.TryDetonate();
    }
}
