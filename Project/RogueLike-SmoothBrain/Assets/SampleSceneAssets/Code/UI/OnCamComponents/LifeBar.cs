using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] Image filledHeart;

    private float lifePourcentage = 0f;
    Hero player;
    public float LifePourcentage
    {
        get { return lifePourcentage; }
        set { lifePourcentage = Mathf.Clamp(value, 0f, 1f); }
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        LifePourcentage = 1f;
        filledHeart.rectTransform.transform.localScale = new Vector3(LifePourcentage, LifePourcentage, LifePourcentage);
    }

    void Update()
    {
        float valueClamped = Mathf.Clamp(player.Stats.GetValue(Stat.HP), 0, player.Stats.GetMaxValue(Stat.HP));
        LifePourcentage = valueClamped / player.Stats.GetMaxValue(Stat.HP);

        filledHeart.rectTransform.transform.localScale = new Vector3(LifePourcentage, LifePourcentage, LifePourcentage);
    }
}
