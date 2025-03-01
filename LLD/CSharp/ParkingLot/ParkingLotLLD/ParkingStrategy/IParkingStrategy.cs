using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Enums;

namespace ParkingLotLLD.ParkingStrategy;

public interface IParkingStrategy
{
    ParkingSpot FindParkingSpot(ParkingSpotEnum parkingSpotEnum);
}