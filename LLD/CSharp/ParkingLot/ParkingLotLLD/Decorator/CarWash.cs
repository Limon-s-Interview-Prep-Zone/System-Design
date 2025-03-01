using ParkingLotLLD.Entities.ParkingSpot;

namespace ParkingLotLLD.Decorator;

public class CarWash : ParkingSpotDecorator
{
    public CarWash(ParkingSpot parkingSpot) : base(parkingSpot)
    {
    }

    public override int Cost(int parkingHours)
    {
        return _parkingSpot.Cost(parkingHours) + 50;
    }
}