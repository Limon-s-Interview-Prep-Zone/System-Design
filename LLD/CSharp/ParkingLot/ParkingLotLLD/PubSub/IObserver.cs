using ParkingLotLLD.Dtos;

namespace ParkingLotLLD.PubSub;

public interface IObserver
{
    void Update(ParkingEvent @event);
}