using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities.Vehicle;

public class Bike:Vehicle
{
    public Bike():base(ParkingSpotEnum.Mini)
    {
    }
}