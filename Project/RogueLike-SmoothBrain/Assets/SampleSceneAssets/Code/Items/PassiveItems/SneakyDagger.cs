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
        Vector3 enemyToPlayerVec = (player.position - (target as MonoBehaviour).transform.position).normalized;

        //if the player is in the back of the enemy, and is in angle behind of 2 * (180 - (180 * 0.85)), it inflicts more damages
        if (Vector3.Dot(enemyToPlayerVec, (target as MonoBehaviour).transform.forward) < -0.85f)
        {
            damages += attackValue;
        }
    }
} 
