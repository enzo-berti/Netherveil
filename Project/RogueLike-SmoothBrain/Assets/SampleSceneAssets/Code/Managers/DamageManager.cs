using UnityEngine;

public class DamageManager : MonoBehaviour
{
    static DamageManager instance;
    public static DamageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load("DamageManager") as GameObject).GetComponent<DamageManager>();
            }
            return instance;
        }
    }

    [SerializeField] DamageText dmgTextPrefab;
    
    public void CreateDamageText(int dmgPt, Vector3 pos)
    {
        DamageText newText = Instantiate(dmgTextPrefab, pos, Quaternion.identity);
        if (dmgPt > 10 && dmgPt < 100)
        {
            //newText.size = dmgPt;
        }
        newText.SetText(dmgPt.ToString());
    }

    public void CreateDamageText(int dmgPt, Vector3 pos, int randPos)
    {
        pos += Random.onUnitSphere * randPos;
        CreateDamageText(dmgPt, pos);
    }
}
