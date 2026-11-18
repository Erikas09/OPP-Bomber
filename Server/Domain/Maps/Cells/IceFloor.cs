namespace Bomber.Domain.Maps.Cells
{
    public class IceFloor : Floor
    {
        public IceFloor()
        {
            Name = "Ice floor";
            Theme = Map.MapType.Ice;
            walkingSpeedModifier = 1.5f;
        }
    }
}
