using Bomber.Domain.Common;
using Bomber.Domain.Common.Interfaces;
using Bomber.Domain.Maps;

namespace Bomber.Domain.Maps.Cells;

public sealed class MapCellIterator : IIterator<MapCell>
{
    private readonly Map _map;

    private int _x;
    private int _y;

    public MapCellIterator(Map map)
    {
        _map = map;
    }

    public bool HasNext()
    {
        return _x < _map.Width;
    }

    public MapCell Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more map cells.");

        GridPosition position = new(_x, _y);
        MapCell cell = _map.GetCell(position);

        _y++;

        if (_y >= _map.Height)
        {
            _y = 0;
            _x++;
        }

        return cell;
    }

    public void Reset()
    {
        _x = 0;
        _y = 0;
    }
}
