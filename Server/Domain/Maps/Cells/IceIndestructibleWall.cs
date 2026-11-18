namespace Bomber.Domain.Maps.Cells
{
    public class IceIndestructibleWall : IndestructibleWall
    {
        public IceIndestructibleWall()
        {
            Name = "Ice indestructible wall";
            Theme = Map.MapType.Ice;
        }

        public override bool CanBeDestroyed => true;
    }
}
