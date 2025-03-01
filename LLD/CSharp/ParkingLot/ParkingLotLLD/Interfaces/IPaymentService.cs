namespace ParkingLotLLD.Interfaces;

public interface IPaymentService
{
    void AcceptCash(int amount);
    void AcceptCreditCard(string cardNumber, int cvv, int amount);
}