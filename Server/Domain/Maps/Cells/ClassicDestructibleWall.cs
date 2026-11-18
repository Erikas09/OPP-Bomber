namespace Bomber.Domain.Maps.Cells
{
    public class ClassicDestructibleWall : DestructibleWall
    {
        public ClassicDestructibleWall()
        {
            Name = "Classic destructible wall";
            Theme = Map.MapType.Classic;
        }
    }
}
