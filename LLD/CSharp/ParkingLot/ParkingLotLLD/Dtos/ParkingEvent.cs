using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Dtos;

public class ParkingEvent
{
    private ParkingEventType _parkingEventType;
    private ParkingSpotEnum _parkingSpotEnum;

    public ParkingEvent(ParkingEventType parkingEventType, ParkingSpotEnum parkingSpotEnum)
    {
        _parkingEventType = parkingEventType;
        _parkingSpotEnum = parkingSpotEnum;
    }

    public  ParkingEventType ParkingEventType
    {
        get => _parkingEventType;
        set => _parkingEventType=value;
    }

    public ParkingSpotEnum ParkingSpotEnum
    {
        get => _parkingSpotEnum;
        set => _parkingSpotEnum = value;
    }
}