using UnityEditor;
using UnityEngine;

static public class FloatingTextGenerator
{
    static readonly int MAX_SIZE = 150;
    static readonly int MIN_SIZE = 50;
    static Color critColor = new Color(0.98f, 0.18f, 0.01f);
    static Color healColor = new Color(0.5f, 0.72f, 0.09f);
    static Color actionColor = new Color(0.75f, 0.75f, 0.75f);

    public static void CreateDamageText(int dmgPt, Vector3 pos, bool isCrit)
    {
        FloatingText newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.identity).GetComponent<FloatingText>();
        newText.SetText(dmgPt.ToString());

        int size = Mathf.Clamp(dmgPt + MIN_SIZE, MIN_SIZE, MAX_SIZE);
        newText.SetSize(size);

        newText.SetColor(isCrit ? critColor : Color.white);
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
        newText.SetColor(healColor);

        int size = Mathf.Clamp(healPt + MIN_SIZE, MIN_SIZE, MAX_SIZE);
        newText.SetSize(size);
    }

    public static void CreatePushedText(Vector3 pos)
    {
        FloatingText newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.Euler(0, 0, 40)).GetComponent<FloatingText>();
        newText.SetText("*PUSHED*");
        newText.SetColor(actionColor);
        newText.SetSize(MIN_SIZE);
    }
}
