using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities.Vehicle;

public abstract class Vehicle
{
    private static int nextId = 0;
    public int Id { get; private set; }
    public string Name { get; private set; }
    public ParkingSpotEnum SupportedParkingSpot { get; private set; }

    public Vehicle(ParkingSpotEnum parkingSpotEnum)
    {
        SupportedParkingSpot = parkingSpotEnum;
        Id = Interlocked.Increment(ref nextId);
    }
    
}