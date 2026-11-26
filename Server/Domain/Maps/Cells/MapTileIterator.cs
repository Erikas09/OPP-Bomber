using Bomber.Domain.Common;
using Bomber.Domain.Common.Interfaces;
using Bomber.Domain.Maps;

namespace Bomber.Domain.Maps.Cells;

public sealed class MapTileIterator : IIterator<MapTile>
{
    private readonly Map _map;

    private int _x;
    private int _y;

    public MapTileIterator(Map map)
    {
        _map = map;
    }

    public bool HasNext()
    {
        return _x < _map.Width;
    }

    public MapTile Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more map tiles.");

        GridPosition position = new(_x, _y);
        MapTile tile = _map.GetTile(position);

        _y++;

        if (_y >= _map.Height)
        {
            _y = 0;
            _x++;
        }

        return tile;
    }

    public void Reset()
    {
        _x = 0;
        _y = 0;
    }
}
