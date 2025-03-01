namespace ParkingLot.API.Entities;

public class CompactParkingSpot : ParkingSpot
{
    public CompactParkingSpot(int floorNumber, int amount = 20) : base(floorNumber, amount)
    {
    }
}