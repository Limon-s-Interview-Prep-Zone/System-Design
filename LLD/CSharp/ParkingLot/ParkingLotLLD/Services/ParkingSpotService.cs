using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;

namespace ParkingLotLLD.Services;

public class ParkingSpotService: IParkingSpotService
{
    private DisplayService _displayService = new DisplayService();
    public ParkingSpot CreateParkingSpot(ParkingSpotEnum parkingSpotEnum, int floorNumber)
    {
        ParkingSpot parkingSpot = ParkingSpotFactory.CreateParkingSpotType(parkingSpotEnum, floorNumber);
        ParkingLot.GetInstance().FreeParkingSpots[parkingSpotEnum].Add(parkingSpot);
        _displayService.Update(parkingSpotEnum, 1);
        return parkingSpot;
    }
}