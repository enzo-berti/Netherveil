
public class TomeOfBenediction : ItemEffect , IPassiveItem 
{
    readonly int value = 15;
    public void OnRetrieved() 
    {
        PlayerController.Get().GetComponent<Hero>().Stats.DecreaseValue(Stat.CORRUPTION, value);
    } 
 
    public void OnRemove() 
    {
        PlayerController.Get().GetComponent<Hero>().Stats.IncreaseValue(Stat.CORRUPTION, value);
    }
} 
