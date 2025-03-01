using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities;

public class DisplayBoard
{
    private static DisplayBoard _displayBoard;
    private static readonly object _lock = new();

    private DisplayBoard()
    {
        FreeParkingSpots = new Dictionary<ParkingSpotEnum, int>();
    }

    public Dictionary<ParkingSpotEnum, int> FreeParkingSpots { get; }

    public static DisplayBoard GetInstance()
    {
        if (_displayBoard == null)
            lock (_lock)
            {
                _displayBoard = _displayBoard ?? new DisplayBoard();
            }

        return _displayBoard;
    }
}