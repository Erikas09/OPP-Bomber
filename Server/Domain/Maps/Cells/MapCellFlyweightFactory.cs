namespace Bomber.Domain.Maps.Cells;

/// <summary>
/// Flyweight Factory. Keeps a pool (cache) of shared <see cref="MapCell"/> instances,
/// one per concrete cell type. If the requested flyweight already exists it is returned,
/// otherwise it is created, cached and returned.
/// </summary>
public static class MapCellFlyweightFactory
{
    private static readonly Dictionary<Type, MapCell> _cache = new();
    private static readonly object _lock = new();

    public static T Get<T>() where T : MapCell, new()
    {
        lock (_lock)
        {
            if (!_cache.TryGetValue(typeof(T), out MapCell? cell))
            {
                cell = new T();
                _cache[typeof(T)] = cell;
            }

            return (T)cell;
        }
    }

    public static int CachedCount
    {
        get
        {
            lock (_lock)
                return _cache.Count;
        }
    }
}
