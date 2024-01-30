using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject weapon;

    private void Update()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.gameObject.GetComponent<Hero>().Stats.GetValueStat(Stat.ATK_RANGE));
    }
    
}
