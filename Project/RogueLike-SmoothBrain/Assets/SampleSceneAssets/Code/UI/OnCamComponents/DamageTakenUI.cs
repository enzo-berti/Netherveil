using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageTakenUI : MonoBehaviour
{
    Image image;
    Color currentColor;
    void Start()
    {
        image = GetComponent<Image>();
        currentColor = image.color;
        Hero.OnTakeDamage += CallTakeDamageUICoroutine;
    }

    private void OnDestroy()
    {
        Hero.OnTakeDamage -= CallTakeDamageUICoroutine;
    }

    private void CallTakeDamageUICoroutine(int damages, IAttacker attacker)
    {
        StartCoroutine(TakeDamageUI());
    }

    IEnumerator TakeDamageUI()
    {
        currentColor.a = 1;
        image.color = currentColor;

        while (image.color.a > 0)
        {
            currentColor.a -= Time.deltaTime;
            image.color = currentColor;
            yield return null;
        }

        yield return null;
    }
}
