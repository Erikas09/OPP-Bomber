namespace Bomber.Domain.Maps.Cells
{
    public class ClassicDestructibleWallSource : DestructibleWallSource
    {
        public ClassicDestructibleWallSource()
        {
            Name = "Classic destructible wall source";
            Theme = Map.MapType.Classic;
        }
    }
}
