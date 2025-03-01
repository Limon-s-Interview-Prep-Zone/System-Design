using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.Vehicle;

namespace ParkingLotLLD.Interfaces;

public interface IParkingService
{
    ParkingTicket Entry(Vehicle vehicle);
    int Exist(ParkingTicket parkingTicket, Vehicle vehicle);
}