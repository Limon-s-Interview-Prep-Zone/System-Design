using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Interfaces;

public interface IDisplayService
{
    void Update(ParkingSpotEnum parkingSpotEnum, int count);
}