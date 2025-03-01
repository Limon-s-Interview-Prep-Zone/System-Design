namespace ParkingLotLLD.Entities;

public class ParkingTicket
{
    private static int nextId=0;

    public int Id { get; private set; }
    public Vehicle.Vehicle Vehicle { get; private set; }
    public ParkingSpot.ParkingSpot ParkingSpot { get; private set; }
    public DateTime Timestamp { get; private set; }

    public ParkingTicket(Vehicle.Vehicle vehicle, ParkingSpot.ParkingSpot parkingSpot)
    {
        Id = Interlocked.Increment(ref nextId);
        Vehicle = vehicle;
        ParkingSpot = parkingSpot;
        Timestamp = DateTime.UtcNow;
    }
}