using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Interfaces;

public interface IParkingSpotService
{
    ParkingSpot CreateParkingSpot(ParkingSpotEnum parkingSpotEnum, int floorNumber);
}