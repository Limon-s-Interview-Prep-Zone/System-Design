namespace ParkingLotLLD.Entities;

public class EntrancePanel
{
    public EntrancePanel(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}