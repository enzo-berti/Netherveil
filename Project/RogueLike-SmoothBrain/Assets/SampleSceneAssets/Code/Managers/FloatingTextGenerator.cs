using UnityEditor;
using UnityEngine;

static public class FloatingTextGenerator
{
    static readonly int MAX_SIZE = 150;
    static readonly int MIN_SIZE = 50;
    static Color critColor = new(0.98f, 0.18f, 0.01f);
    static Color healColor = new(0.5f, 0.72f, 0.09f);
    static Color actionColor = new(0.75f, 0.75f, 0.75f);

    public static void CreateDamageText(int dmgPt, Vector3 pos, bool isCrit = false, int randPos = 1)
    {
        CreateNumberText(dmgPt, pos, Color.white, out FloatingText newText, randPos);
        newText.SetColor(isCrit ? critColor : Color.white);
    }

    public static void CreateHealText(int healPt, Vector3 pos, int randPos)
    {
        CreateNumberText(healPt, pos, healColor, out _, randPos);
    }

    public static void CreateActionText(Vector3 pos, string text, int randPos = 1)
    {
        pos += Vector3.up * 2;
        Vector3 randomOffsetVec = Random.onUnitSphere * randPos;
        pos += new Vector3(randomOffsetVec.x, 0f, randomOffsetVec.z);

        FloatingText newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.identity).GetComponent<FloatingText>();
        newText.SetText(text);
        newText.SetColor(actionColor);
        newText.SetSize(MIN_SIZE);
    }

    private static void CreateNumberText(int nb, Vector3 pos, Color color, out FloatingText newText, int randPos)
    {
        pos += Vector3.up * 2;
        pos += Random.onUnitSphere * randPos;

        newText = GameObject.Instantiate(Resources.Load("FloatingText") as GameObject, pos, Quaternion.identity).GetComponent<FloatingText>();
        newText.SetText(nb.ToString());
        newText.SetColor(color);

        int size = Mathf.Clamp(nb + MIN_SIZE, MIN_SIZE, MAX_SIZE);
        newText.SetSize(size);
    }
}
