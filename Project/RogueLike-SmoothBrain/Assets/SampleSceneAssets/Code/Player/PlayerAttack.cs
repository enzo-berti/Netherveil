using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject weapon;

    private void Update()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.gameObject.GetComponent<Hero>().Stats.GetValueStat(Stat.ATK_RANGE));
    }
    public void ThrowSpear()
    {
        if(!GetComponent<PlayerInput>().LaunchedAttack)
        {
            Spear spear = weapon.GetComponent<Spear>();

            // If spear is being thrown we can't recall this attack
            if (spear.IsThrowing) return;
            if (!spear.IsThrown)
            {
                spear.Throw(this.transform.position + this.transform.forward * this.gameObject.GetComponent<Hero>().Stats.GetValueStat(Stat.ATK_RANGE));
            }
            else
            {
                spear.Return();
            }
        }
    }
}
