public interface IConsommable
{
    public float Price
    {
        get;
    }

    public bool CanBeRetreived
    {
        get;
    }
    void OnRetreived();
}
