namespace ParkingLotLLD.Entities.ParkingSpot;

public class LargeParkingSpot:ParkingSpot
{
    public LargeParkingSpot(int floorNumber, int amount=40) : base(floorNumber, amount)
    {
    }

    public override int Cost(int parkingHours)
    {
        return parkingHours * Amount;
    }
}