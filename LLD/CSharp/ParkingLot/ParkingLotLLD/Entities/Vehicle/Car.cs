using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities.Vehicle;

public class Car: Vehicle
{
    public Car() : base(ParkingSpotEnum.Compact)
    {
    }
}