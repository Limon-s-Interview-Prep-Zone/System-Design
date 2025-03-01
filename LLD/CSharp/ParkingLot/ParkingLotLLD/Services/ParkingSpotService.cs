using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;

namespace ParkingLotLLD.Services;

public class ParkingSpotService : IParkingSpotService
{
    private readonly DisplayService _displayService = new();

    public ParkingSpot CreateParkingSpot(ParkingSpotEnum parkingSpotEnum, int floorNumber)
    {
        var parkingSpot = ParkingSpotFactory.CreateParkingSpotType(parkingSpotEnum, floorNumber);
        ParkingLot.GetInstance().FreeParkingSpots[parkingSpotEnum].Add(parkingSpot);
        _displayService.Update(parkingSpotEnum, 1);
        return parkingSpot;
    }
}