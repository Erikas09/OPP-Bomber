namespace Bomber.Domain.Maps.Cells
{
    public class ClassicIndestructibleWall : IndestructibleWall
    {
        public ClassicIndestructibleWall()
        {
            Name = "Classic indestructible wall";
            Theme = Map.MapType.Classic;
        }

        public override bool CanBeDestroyed => true;
    }
}
