namespace ParkingLot.API.Entities;

public class MiniParkingSpot : ParkingSpot
{
    public MiniParkingSpot(int floorNumber, int amount = 10) : base(floorNumber, amount)
    {
    }
}