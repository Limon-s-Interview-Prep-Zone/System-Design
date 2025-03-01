using ParkingLotLLD.Entities.ParkingSpot;

namespace ParkingLotLLD.Decorator;

public class CarCharge: ParkingSpotDecorator
{
    public CarCharge(ParkingSpot parkingSpot) : base(parkingSpot)
    {
    }

    public override int Cost(int parkingHours)
    {
        return _parkingSpot.Cost(parkingHours)+ parkingHours*50;
    }
}