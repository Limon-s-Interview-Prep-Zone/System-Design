using ParkingLotLLD.Entities;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;

namespace ParkingLotLLD.Services;

public class DisplayService: IDisplayService
{
    public void Update(ParkingSpotEnum parkingSpotEnum, int count)
    {
        if (!DisplayBoard.GetInstance().FreeParkingSpots.TryGetValue(parkingSpotEnum, out var value))
        {
            DisplayBoard.GetInstance().FreeParkingSpots[parkingSpotEnum] = 1;
            return;
        }
        DisplayBoard.GetInstance().FreeParkingSpots[parkingSpotEnum] += count;
    }
}