namespace Bomber.Domain.Maps.Cells
{
    public class ClassicFloor : Floor
    {
        public ClassicFloor()
        {
            Name = "Classic floor";
            Theme = Map.MapType.Classic;
            walkingSpeedModifier = 1f;
        }
    }
}
