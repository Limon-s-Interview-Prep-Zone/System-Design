using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities.Vehicle;

public class Truck: Vehicle
{
    public Truck( ) : base(ParkingSpotEnum.Large)
    {
    }
}