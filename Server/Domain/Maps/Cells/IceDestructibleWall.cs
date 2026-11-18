namespace Bomber.Domain.Maps.Cells
{
    public class IceDestructibleWall : DestructibleWall
    {
        public IceDestructibleWall()
        {
            Name = "Ice destructible wall";
            Theme = Map.MapType.Ice;
        }
    }
}
