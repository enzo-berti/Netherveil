using UnityEngine;

public class SneakyDagger : ItemEffect , IPassiveItem 
{
    readonly int attackValue = 15;
    public void OnRetrieved() 
    {
        Hero.OnBeforeApplyDamages += ExtraSneakyDamages;
    }

    public void OnRemove() 
    {
        Hero.OnBeforeApplyDamages -= ExtraSneakyDamages;
    }

    private void ExtraSneakyDamages(ref int damages, IDamageable target)
    {
        Transform player = GameObject.FindWithTag("Player").transform;

        if(Vector3.Dot(player.position - (target as MonoBehaviour).transform.position, (target as MonoBehaviour).transform.forward) < 0)
        {
            damages += attackValue;
        }
    }
} 
