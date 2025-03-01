using ParkingLotLLD.Enums;

namespace ParkingLotLLD.Entities.ParkingSpot;

public class ParkingSpotFactory
{
    // TODO: need to make this dynamic
    private static readonly Dictionary<ParkingSpotEnum, Type?> ParkingSpotTypes =
        new Dictionary<ParkingSpotEnum, Type?>()
        {
            [ParkingSpotEnum.Mini]= typeof(MiniParkingSpot),
            [ParkingSpotEnum.Compact]= typeof(CompactParkingSpot),
            [ParkingSpotEnum.Large]= typeof(LargeParkingSpot)
        };

    public static ParkingSpot CreateParkingSpotType(ParkingSpotEnum parkingSpotEnum, int floorNumber)
    {
        if (ParkingSpotTypes.TryGetValue(parkingSpotEnum, out Type? parkingSpotType))
        {
            return (ParkingSpot)Activator.CreateInstance(parkingSpotType!, new object[] {floorNumber,100})!;
        }
        throw new ArgumentException("Invalid parking spot type", nameof(parkingSpotEnum));
    }
}