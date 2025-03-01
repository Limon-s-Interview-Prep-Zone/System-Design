namespace ParkingLot.API.Entities;

public abstract class ParkingSpot
{
    private static int nextId=0;
    public int Id { get; private set; }
    public int FloorNumber { get; private set; }
    public int Amount { get; private set; }
    public bool IsFree { get; private set; } = true;

    protected ParkingSpot(int floorNumber, int amount)
    {
        FloorNumber = floorNumber;
        Amount = amount;
        Id = Interlocked.Increment(ref nextId);
    }

    public bool SetIsFree
    {
        set => IsFree = value;
    }
}