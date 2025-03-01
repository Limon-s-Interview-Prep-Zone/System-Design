using ParkingLotLLD.Decorator;
using ParkingLotLLD.Dtos;
using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Entities.Vehicle;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;
using ParkingLotLLD.ParkingStrategy;
using ParkingLotLLD.PubSub;

namespace ParkingLotLLD.Services;

public class ParkingService: IParkingService
{
    private IParkingStrategy _parkingStrategy;
    private readonly DisplayService _displayService;
    private readonly ParkingLot _parkingLot;
    private static object _lock= new object();
    private IEventBus _eventBus;

    public ParkingService(IParkingStrategy parkingStrategy)
    {
        _parkingStrategy = parkingStrategy;
        _displayService = new DisplayService();
        _parkingLot = ParkingLot.GetInstance();
        _eventBus = new EventBus();
        _eventBus.Subscribe(_displayService);
    }
    public ParkingTicket Entry(Vehicle vehicle)
    {
        ParkingSpotEnum parkingSpotEnum = vehicle.SupportedParkingSpot;
        ParkingSpot parkingSpot = _parkingStrategy.FindParkingSpot(parkingSpotEnum);
        try
        {
            if (parkingSpot.IsFree)
            {
                lock (_lock)
                {
                    List<ParkingSpot> freeParkingSpots = ParkingLot.GetInstance().FreeParkingSpots[parkingSpotEnum];
                    List<ParkingSpot> occupiedParkingSpots = ParkingLot.GetInstance().OccupiedParkingSpots[parkingSpotEnum];

                    if (parkingSpot.IsFree)
                    {
                        parkingSpot.SetIsFree = false;
                        freeParkingSpots.Remove(parkingSpot);
                        occupiedParkingSpots.Add(parkingSpot);
                        ParkingTicket parkingTicket = new ParkingTicket(vehicle, parkingSpot);
                        ParkingEvent parkingEvent = new ParkingEvent(ParkingEventType.ENTRY, parkingSpotEnum);
                        _eventBus.Publish(parkingEvent);
                        // _displayService.Update(parkingSpotEnum, -1);
                        return parkingTicket;   
                    }
                    Entry(vehicle);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return default;
    }

    private void AddParkingSpotInFreeList(List<ParkingSpot> parkingSpots, ParkingSpot parkingSpot)
    {
        // int correctIndex = BinarySearchInsertIndex()
        int l = 0, r = parkingSpots.Count - 1;
        while (l<=r)
        {
            int mid = l + (r - l) / 2;
            if (parkingSpots[mid].Id < parkingSpot.Id)
            {
                l = mid + 1;
            }else if (parkingSpots[mid].Id > parkingSpot.Id)
            {
                r = mid - 1;
            }
            else
            {
                return;
            }
        }
        parkingSpots.Insert(l, parkingSpot);
    }
    public int Exist(ParkingTicket parkingTicket, Vehicle vehicle)
    {
        if (parkingTicket.Vehicle.Equals(vehicle))
        {
            ParkingSpot parkingSpot = parkingTicket.ParkingSpot;
            int amount = parkingSpot.Amount;
            parkingSpot.SetIsFree = true;
            _parkingLot.OccupiedParkingSpots[vehicle.SupportedParkingSpot].Remove(parkingSpot);
            // set the parkingSpot on the appropriate position
            AddParkingSpotInFreeList(_parkingLot.FreeParkingSpots[vehicle.SupportedParkingSpot], parkingSpot);
            
            
            _eventBus.Publish(new ParkingEvent(ParkingEventType.EXIST, vehicle.SupportedParkingSpot));
            // _displayService.Update(vehicle.SupportedParkingSpot, 1);
            return amount;
        }
        throw new NotImplementedException();
    }
    
    // TODO: separate this into another service
    public void AddWash(ParkingTicket parkingTicket)
    {
        parkingTicket.ParkingSpot = new CarWash(parkingTicket.ParkingSpot);
    }
}