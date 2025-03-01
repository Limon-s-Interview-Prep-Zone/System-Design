namespace ParkingLot.API.Entities;

public class LargeParkingSpot : ParkingSpot
{
    public LargeParkingSpot(int floorNumber, int amount = 40) : base(floorNumber, amount)
    {
    }
}