// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.ParkingSpot;
using ParkingLotLLD.Entities.Vehicle;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;
using ParkingLotLLD.ParkingStrategy;
using ParkingLotLLD.Services;

ParkingLot parkingLot = ParkingLot.GetInstance();
IParkingSpotService parkingSpotService = new ParkingSpotService();

// create parkingSpot

ParkingSpot c1= parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Compact, 101);
ParkingSpot c2= parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Compact, 101);

ParkingSpot m1= parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Mini, 101);
ParkingSpot m2= parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Mini, 101);

ParkingSpot l1= parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Large, 101);
ParkingSpot l2= parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Large, 101);

// create vehicle
Vehicle bike1 = new Bike();
Vehicle bike2 = new Bike();
Vehicle bike3 = new Bike();
DisplayBoard displayBoard = DisplayBoard.GetInstance();
Console.WriteLine($"Free Parking Spot:: {JsonSerializer.Serialize(displayBoard.FreeParkingSpots)}");

IParkingService parkingService = new ParkingService(new NearestFirstParkingStrategy());
IPaymentService paymentService = new PaymentService();

ParkingTicket parkingTicket1 = parkingService.Entry(bike1);
Console.WriteLine($"Parking Ticket1 with ${JsonSerializer.Serialize(parkingTicket1)}");

Console.WriteLine($"Free Parking Spot:: {JsonSerializer.Serialize(displayBoard.FreeParkingSpots)}");

try
{
    parkingService.Exist(parkingTicket1, bike1);
    int cost = parkingTicket1.ParkingSpot.Amount;
    Console.WriteLine($"Total cost: {cost}");
    paymentService.AcceptCash(cost);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
Console.WriteLine($"Free Parking Spot:: {JsonSerializer.Serialize(displayBoard.FreeParkingSpots)}");





