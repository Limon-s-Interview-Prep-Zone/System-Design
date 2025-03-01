namespace ParkingLotLLD.Entities.Accounts;

public abstract class Account {
    protected Account(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

}