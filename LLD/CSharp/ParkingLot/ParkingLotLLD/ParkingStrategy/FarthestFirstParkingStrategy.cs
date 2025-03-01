using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Enums;

namespace ParkingLotLLD.ParkingStrategy;

public class FarthestFirstParkingStrategy : IParkingStrategy
{
    public ParkingSpot FindParkingSpot(ParkingSpotEnum parkingSpotEnum)
    {
        var parkingSpots = ParkingLot.GetInstance().FreeParkingSpots[parkingSpotEnum];
        if (parkingSpots.Count < 1)
        {
            Console.WriteLine("Spot not found in the farthest first strategy");
            throw new Exception($"Spot not found in the farthest first strategy for ${parkingSpotEnum}");
        }

        return parkingSpots[^1];
    }
}