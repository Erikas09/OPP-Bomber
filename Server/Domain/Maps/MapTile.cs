using Bomber.Domain.Common;
using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Maps;

/// <summary>
/// Flyweight Context. Holds the extrinsic (unique) state of one map tile
/// (its id and position) together with a reference to a shared <see cref="MapCell"/> flyweight.
/// </summary>
public sealed class MapTile
{
    public Guid Id { get; }
    public GridPosition Position { get; }
    public MapCell Cell { get; }

    public MapTile(GridPosition position, MapCell cell)
    {
        Id = Guid.NewGuid();
        Position = position;
        Cell = cell;
    }

    public bool IsWalkable => Cell.IsWalkable;

    public string Describe()
    {
        return Cell.Describe(Position);
    }
}
