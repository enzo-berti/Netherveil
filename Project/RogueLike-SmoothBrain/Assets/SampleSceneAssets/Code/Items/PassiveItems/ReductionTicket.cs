using UnityEngine; 
 
public class ReductionTicket : ItemEffect , IPassiveItem 
{
    readonly float coefValue = 0.5f;

#pragma warning disable CS0414 // Supprimer le warning dans unity
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
    //used to display in description, dont delete it
    private readonly float displayValue = 0f;
#pragma warning restore IDE0052
#pragma warning restore CS0414

    public ReductionTicket()
    {
        displayValue = coefValue * 100f;
    }

    public void OnRetrieved() 
    {
        Item.priceCoef *= coefValue;
    }

    public void OnRemove() 
    {
        Item.priceCoef /= coefValue;
    } 
 
} 
