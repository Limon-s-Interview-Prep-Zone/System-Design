using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities;

public class DisplayBoard
{
    private static DisplayBoard _displayBoard = null;
    private static readonly object _lock = new object();
    private Dictionary<ParkingSpotEnum, int> _freeParkingSpots;

    private DisplayBoard()
    {
        _freeParkingSpots = new Dictionary<ParkingSpotEnum, int>();
    }

    public static DisplayBoard GetInstance()
    {
        if (_displayBoard == null)
        {
            lock(_lock)
            {
                _displayBoard = _displayBoard??new DisplayBoard();
            }
        }

        return _displayBoard;
    }

    public Dictionary<ParkingSpotEnum, int> FreeParkingSpots
    {
        get { return _freeParkingSpots; }
    }

}