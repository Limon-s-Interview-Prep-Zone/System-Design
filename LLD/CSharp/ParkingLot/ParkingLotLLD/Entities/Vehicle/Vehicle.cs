using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities.Vehicle;

public abstract class Vehicle
{
    private static int nextId;

    public Vehicle(ParkingSpotEnum parkingSpotEnum)
    {
        SupportedParkingSpot = parkingSpotEnum;
        Id = Interlocked.Increment(ref nextId);
    }

    public int Id { get; private set; }
    public string Name { get; }
    public ParkingSpotEnum SupportedParkingSpot { get; private set; }
}