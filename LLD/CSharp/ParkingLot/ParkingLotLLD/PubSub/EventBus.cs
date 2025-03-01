using ParkingLotLLD.Dtos;

namespace ParkingLotLLD.PubSub;

public class EventBus : IEventBus
{
    private static readonly List<IObserver> _observers = new();

    public void Subscribe(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnSubscribe(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Publish(ParkingEvent @event)
    {
        _observers.ForEach(observer =>
            observer.Update(@event));
    }
}