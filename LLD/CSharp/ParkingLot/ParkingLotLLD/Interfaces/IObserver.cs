using ParkingLotLLD.Dtos;

namespace ParkingLotLLD.Interfaces;

public interface IObserver
{
    void Update(ParkingEvent @event);
}