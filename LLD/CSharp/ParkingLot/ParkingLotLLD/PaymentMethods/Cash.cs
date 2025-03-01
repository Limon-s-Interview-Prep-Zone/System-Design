namespace ParkingLotLLD.PaymentMethods;

public class Cash: PaymentMethod
{
    public override bool InitPayment(int amount)
    {
        Console.WriteLine($"making payment by cash of  BDT {amount}");
        return true;
    }
}