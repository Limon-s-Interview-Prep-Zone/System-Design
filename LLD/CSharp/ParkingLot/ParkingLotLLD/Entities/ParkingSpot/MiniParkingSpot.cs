namespace ParkingLotLLD.Entities.ParkingSpot;

public class MiniParkingSpot : ParkingSpot
{
    public MiniParkingSpot(int floorNumber, int amount = 10) : base(floorNumber, amount)
    {
    }

    public override int Cost(int parkingHours)
    {
        return parkingHours * Amount;
    }
}