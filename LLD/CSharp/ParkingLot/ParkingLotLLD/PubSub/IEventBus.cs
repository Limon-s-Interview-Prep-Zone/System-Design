using ParkingLotLLD.Dtos;

namespace ParkingLotLLD.PubSub;

public interface IEventBus
{
    void Subscribe(IObserver observer);
    void UnSubscribe(IObserver observer);
    void Publish(ParkingEvent @event);
}