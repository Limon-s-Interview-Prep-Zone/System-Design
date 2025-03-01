using ParkingLotLLD.Entities.ParkingSpot;

namespace ParkingLotLLD.Decorator;

public abstract class ParkingSpotDecorator: ParkingSpot
{
    protected ParkingSpot _parkingSpot;
    protected ParkingSpotDecorator(ParkingSpot parkingSpot)
    {
        _parkingSpot = parkingSpot;
    }
}