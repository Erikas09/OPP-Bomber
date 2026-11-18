using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

/// <summary>
/// Flyweight. Holds only the intrinsic (shared) state of a map cell type:
/// name, theme, walkability. Instances are shared between many tiles via
/// <see cref="MapCellFlyweightFactory"/>, so they are immutable and contain no
/// tile-specific data (position, id). That extrinsic state lives in MapTile.
/// </summary>
public abstract class MapCell
{
    // Intrinsic state (repeatingState) – set once in the concrete constructor.
    public string Name { get; protected init; } = string.Empty;
    public Map.MapType Theme { get; protected init; }

    public abstract bool IsWalkable { get; }

    /// <summary>
    /// Flyweight operation: extrinsic state (position) is passed in by the caller.
    /// </summary>
    public string Describe(GridPosition position)
    {
        return $"{Name} ({Theme}) at ({position.X}, {position.Y})";
    }
}
