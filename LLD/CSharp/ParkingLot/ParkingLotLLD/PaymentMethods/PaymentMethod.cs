namespace ParkingLotLLD.PaymentMethods;

public abstract class PaymentMethod
{
    public abstract bool InitPayment(int amount);
}