using ParkingLotLLD.Dtos;
using ParkingLotLLD.Entities;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;
using ParkingLotLLD.PubSub;

namespace ParkingLotLLD.Services;

public class DisplayService: IDisplayService, IObserver
{
    public void Update(ParkingSpotEnum parkingSpotEnum, int count)
    {
        if (!DisplayBoard.GetInstance().FreeParkingSpots.TryGetValue(parkingSpotEnum, out var value))
        {
            DisplayBoard.GetInstance().FreeParkingSpots[parkingSpotEnum] = 1;
            return;
        }
        DisplayBoard.GetInstance().FreeParkingSpots[parkingSpotEnum] += count;
    }

    public void Update(ParkingEvent @event)
    {
        if (!DisplayBoard.GetInstance().FreeParkingSpots.TryGetValue(@event.ParkingSpotEnum, out var value))
        {
            DisplayBoard.GetInstance().FreeParkingSpots[@event.ParkingSpotEnum] = 1;
            return;
        }
        int change = 0;
        if (@event.ParkingEventType.Equals(ParkingEventType.ENTRY))
        {
            change=-1;
        }
        else
        {
            change=1;
        }
        DisplayBoard.GetInstance().FreeParkingSpots[@event.ParkingSpotEnum] += change;
    }
}