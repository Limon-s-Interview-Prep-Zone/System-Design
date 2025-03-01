using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities;

public class ParkingLot
{
    private string Name;
    private List<EntrancePanel> _entrancePanels;
    private List<ExistPanel> _existPanels;
    private DisplayBoard _displayBoard;
    private Dictionary<ParkingSpotEnum , List<ParkingSpot.ParkingSpot> > _freeParkingSpots;
    private Dictionary<ParkingSpotEnum , List<ParkingSpot.ParkingSpot> > _occupiedParkingSpots;

    private static ParkingLot _parkingLot = null;
    private static readonly object _lock = new object();

    private ParkingLot(string name)
    {
        Name = name;
        _entrancePanels = new List<EntrancePanel>();
        _existPanels = new List<ExistPanel>();
        _displayBoard = DisplayBoard.GetInstance();
        
        // TODO: should be dynamic
        _freeParkingSpots = new Dictionary<ParkingSpotEnum, List<ParkingSpot.ParkingSpot>>()
        {
            [ParkingSpotEnum.Mini]=new List<ParkingSpot.ParkingSpot>(),
            [ParkingSpotEnum.Compact]=new List<ParkingSpot.ParkingSpot>(),
            [ParkingSpotEnum.Large]=new List<ParkingSpot.ParkingSpot>()
        };
        
        // TODO: should be dynamic
        _occupiedParkingSpots = new Dictionary<ParkingSpotEnum, List<ParkingSpot.ParkingSpot>>()
        {
            [ParkingSpotEnum.Mini]=new List<ParkingSpot.ParkingSpot>(),
            [ParkingSpotEnum.Compact]=new List<ParkingSpot.ParkingSpot>(),
            [ParkingSpotEnum.Large]=new List<ParkingSpot.ParkingSpot>(),
        };
    }
    public static ParkingLot GetInstance()
    {
        if (_parkingLot == null)
        {
            lock (_lock)
            {
                _parkingLot = _parkingLot ?? new ParkingLot("ABC");
            }
        }
        return _parkingLot;
    }
    public List<EntrancePanel> EntrancePanels
    {
        get => _entrancePanels;
        set => _entrancePanels = value ?? throw new ArgumentNullException(nameof(value));
    }

    public List<ExistPanel> ExistPanels
    {
        get => _existPanels;
        set => _existPanels = value ?? throw new ArgumentNullException(nameof(value));
    }

    public DisplayBoard DisplayBoard
    {
        get => _displayBoard;
        set => _displayBoard = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Dictionary<ParkingSpotEnum, List<ParkingSpot.ParkingSpot>> FreeParkingSpots
    {
        get => _freeParkingSpots;
        set => _freeParkingSpots = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Dictionary<ParkingSpotEnum, List<ParkingSpot.ParkingSpot>> OccupiedParkingSpots
    {
        get => _occupiedParkingSpots;
        set => _occupiedParkingSpots = value ?? throw new ArgumentNullException(nameof(value));
    }
}