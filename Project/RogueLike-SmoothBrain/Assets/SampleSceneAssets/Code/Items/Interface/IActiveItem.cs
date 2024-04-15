public interface IActiveItem : IItem
{
    float Cooldown { get; set; }
    void OnActive();
}
