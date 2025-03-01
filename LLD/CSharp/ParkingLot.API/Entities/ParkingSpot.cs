namespace ParkingLot.API.Entities;

public abstract class ParkingSpot
{
    private static int nextId;

    protected ParkingSpot(int floorNumber, int amount)
    {
        FloorNumber = floorNumber;
        Amount = amount;
        Id = Interlocked.Increment(ref nextId);
    }

    public int Id { get; private set; }
    public int FloorNumber { get; private set; }
    public int Amount { get; private set; }
    public bool IsFree { get; private set; } = true;

    public bool SetIsFree
    {
        set => IsFree = value;
    }
}