namespace ParkingLotLLD.Entities.ParkingSpot;

public class CompactParkingSpot: ParkingSpot
{
    public CompactParkingSpot(int floorNumber, int amount=20) : base(floorNumber, amount)
    {
    }
    public override int Cost(int parkingHours)
    {
        return parkingHours * Amount;
    }
}