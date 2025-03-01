using ParkingLotLLD.Interfaces;
using ParkingLotLLD.PaymentMethods;

namespace ParkingLotLLD.Services;

public class PaymentService: IPaymentService
{
    public void AcceptCash(int amount)
    {
        PaymentMethod cash = new Cash();
        cash.InitPayment(amount);
    }

    public void AcceptCreditCard(string cardNumber, int cvc, int amount)
    {
        PaymentMethod creditCard = new CreditCard(cardNumber, cvc);
        creditCard.InitPayment(amount);
    }
}