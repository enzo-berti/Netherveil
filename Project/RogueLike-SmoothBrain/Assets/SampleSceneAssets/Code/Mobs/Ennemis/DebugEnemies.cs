using System.Linq;
using TMPro;
using UnityEngine;

public class DebugEnemies : MonoBehaviour
{
    private enum mobs
    {
        MELEE,
        RANGE,
        KAMIKAZE,
        TANK
    }

    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Transform plane;
    private bool[] state;
    private KeyCode[] keys;
    private GameObject[] mobInstances;

    private void Start()
    {
        state = new bool[texts.Count()];
        keys = new KeyCode[texts.Count()];
        keys[0] = KeyCode.Alpha1;
        keys[1] = KeyCode.Alpha2;
        keys[2] = KeyCode.Alpha3;
        keys[3] = KeyCode.Alpha4;

        mobInstances = new GameObject[texts.Count()];
    }

    private void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                state[i] = !state[i];
                UpdateText(state[i], i);
            }
        }
    }

    void UpdateText(bool _state, int i)
    {
        string[] mobs = new string[] { "Melee", "Range", "Kamikaze", "Tank" };

        string text;

        text = _state ? "Destroy " : "Spawn ";
        texts[i].text = "[" + (i + 1) + "] " + text + mobs[i];
        texts[i].color = _state ? Color.red : Color.green;

        if (_state)
        {
            mobInstances[i] = Instantiate(prefabs[i], plane.position, Quaternion.identity);
        }
        else
        {
            Destroy(mobInstances[i]);
        }
    }
}