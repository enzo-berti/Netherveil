public interface IConsommable
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
