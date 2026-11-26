namespace Bomber.Domain.Maps.Cells
{
    public class IceDestructibleWallSource : DestructibleWallSource
    {
        public IceDestructibleWallSource()
        {
            Name = "Ice destructible wall source";
            Theme = Map.MapType.Ice;
        }
    }
}
