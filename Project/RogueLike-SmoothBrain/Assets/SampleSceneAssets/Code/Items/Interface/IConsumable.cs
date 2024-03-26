public interface IConsumable
{
    public float Price
    {
        get;
    }

    public bool CanBeRetrieved
    {
        get;
    }
    void OnRetrieved();
}
