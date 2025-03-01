// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using ParkingLotLLD.Entities;
using ParkingLotLLD.Entities.Vehicle;
using ParkingLotLLD.Enums;
using ParkingLotLLD.Interfaces;
using ParkingLotLLD.ParkingStrategy;
using ParkingLotLLD.Services;

var parkingLot = ParkingLot.GetInstance();
IParkingSpotService parkingSpotService = new ParkingSpotService();

// create parkingSpot

var c1 = parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Compact, 101);
var c2 = parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Compact, 101);

var m1 = parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Mini, 101);
var m2 = parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Mini, 101);

var l1 = parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Large, 101);
var l2 = parkingSpotService.CreateParkingSpot(ParkingSpotEnum.Large, 101);

// create vehicle
Vehicle bike1 = new Bike();
Vehicle bike2 = new Bike();
Vehicle bike3 = new Bike();
var displayBoard = DisplayBoard.GetInstance();
Console.WriteLine($"Free Parking Spot:: {JsonSerializer.Serialize(displayBoard.FreeParkingSpots)}");

var parkingService = new ParkingService(new NearestFirstParkingStrategy());
// add observer
IPaymentService paymentService = new PaymentService();

var parkingTicket1 = parkingService.Entry(bike1);
Console.WriteLine($"Parking Ticket1 with ${JsonSerializer.Serialize(parkingTicket1)}");

Console.WriteLine($"Free Parking Spot:: {JsonSerializer.Serialize(displayBoard.FreeParkingSpots)}");

parkingService.AddWash(parkingTicket1);

try
{
    parkingService.Exist(parkingTicket1, bike1);
    var cost = parkingTicket1.ParkingSpot.Cost(2);
    Console.WriteLine($"Total cost: {cost}");
    paymentService.AcceptCash(cost);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

Console.WriteLine($"Free Parking Spot:: {JsonSerializer.Serialize(displayBoard.FreeParkingSpots)}");