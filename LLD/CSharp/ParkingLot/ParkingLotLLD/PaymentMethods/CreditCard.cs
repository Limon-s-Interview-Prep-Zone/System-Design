namespace ParkingLotLLD.PaymentMethods;

public class CreditCard : PaymentMethod
{
    private string creditCard;
    private int cvc;

    public CreditCard(string creditCard, int cvc)
    {
        this.creditCard = creditCard;
        this.cvc = cvc;
    }

    public override bool InitPayment(int amount)
    {
        // TODO: added payment gateway
        return false;
    }
}