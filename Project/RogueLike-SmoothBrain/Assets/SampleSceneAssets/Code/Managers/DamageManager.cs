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

    public void CreateDamageText(int dmgPt, Vector3 pos, bool isCrit)
    {
        DamageText newText = Instantiate(dmgTextPrefab, pos, Quaternion.identity);
        newText.SetText(dmgPt.ToString());
        if (dmgPt > 10 && dmgPt < 100)
        {
            newText.SetSize(dmgPt + 50);
        }
        else if (dmgPt > 100)
        {
            newText.SetSize(150);
        }
        else if (dmgPt < 10)
        {
            newText.SetSize(50);
        }

        if (isCrit)
        {
            newText.SetColor(new Color(0.98f, 0.18f, 0.01f));//orange
        }
        else
        {
            newText.SetColor(Color.white);
        }
    }

    public void CreateDamageText(int dmgPt, Vector3 pos, bool isCrit, int randPos)
    {
        pos += Random.onUnitSphere * randPos;
        CreateDamageText(dmgPt, pos, isCrit);
    }

    public void CreateHealText(int healPt, Vector3 pos)
    {
        DamageText newText = Instantiate(dmgTextPrefab, pos, Quaternion.identity);
        newText.SetText(healPt.ToString());
        newText.SetColor(new Color(0.5f, 0.72f, 0.09f));//green  //(0.95f, 0.89f, 0.03f));//yellow
        if (healPt > 10 && healPt < 100)
        {
            newText.SetSize(healPt + 50);
        }
        else if (healPt > 100)
        {
            newText.SetSize(150);
        }
        else if (healPt < 10)
        {
            newText.SetSize(50);
        }
    }

    public void CreatePushedText(Vector3 pos)
    {
        DamageText newText = Instantiate(dmgTextPrefab, pos, Quaternion.Euler(0, 0, 40));
        newText.SetText("*PUSHED*");
        newText.SetColor(new Color(0.75f, 0.75f, 0.75f));//grey
        newText.SetSize(50);
    }
}
