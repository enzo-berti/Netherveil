using UnityEngine;

static public class FloatingTextGenerator
{
    public static void CreateDamageText(int dmgPt, Vector3 pos, bool isCrit)
    {
        FloatingText newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.identity).GetComponent<FloatingText>();
        newText.SetText(dmgPt.ToString());
        if (dmgPt >= 10 && dmgPt < 100)
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

    public static void CreateDamageText(int dmgPt, Vector3 pos, bool isCrit = false, int randPos = 1)
    {
        pos += Vector3.up * 2;
        pos += Random.onUnitSphere * randPos;
        CreateDamageText(dmgPt, pos, isCrit);
    }

    public static void CreateHealText(int healPt, Vector3 pos)
    {
        FloatingText newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.identity).GetComponent<FloatingText>();
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

    public static void CreatePushedText(Vector3 pos)
    {
        FloatingText newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.Euler(0, 0, 40)).GetComponent<FloatingText>();
        newText.SetText("*PUSHED*");
        newText.SetColor(new Color(0.75f, 0.75f, 0.75f));//grey
        newText.SetSize(50);
    }
}
