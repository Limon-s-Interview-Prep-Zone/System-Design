using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Dtos;

public class ParkingEvent
{
    public ParkingEvent(ParkingEventType parkingEventType, ParkingSpotEnum parkingSpotEnum)
    {
        ParkingEventType = parkingEventType;
        ParkingSpotEnum = parkingSpotEnum;
    }

    public ParkingEventType ParkingEventType { get; set; }

    public ParkingSpotEnum ParkingSpotEnum { get; set; }
}