namespace ParkingLotLLD.Interfaces;

public interface IPaymentService
{
    void AcceptCash(int amount);
    void AcceptCreditCard(String cardNumber, int cvv, int amount);
}