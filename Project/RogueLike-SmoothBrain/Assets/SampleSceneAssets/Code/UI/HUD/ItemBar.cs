using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour
{
    private Hero hero;
    [SerializeField] private GameObject framePf;
    [SerializeField] private ItemDatabase database;
    private int maxItemDisplay = 5;
    [SerializeField] private Transform itemPassiveTransform;
    [SerializeField] private GameObject activeFrame;
    [SerializeField] private GameObject specialAbilityFrame;
    [SerializeField] private Texture damnationVeilIcon;
    [SerializeField] private Texture divineShieldIcon;

    private void Start()
    {
        hero = FindObjectOfType<Hero>();
    }

    private void OnEnable()
    {
        Item.OnRetrieved += OnItemAdd;
        Hero.OnBenedictionMaxUpgrade += OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxUpgrade += OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxDrawback += OnSpecialAbilityRemove;
        Hero.OnBenedictionMaxDrawback += OnSpecialAbilityRemove;
        IActiveItem.OnActiveItemCooldownStarted += () => StartCoroutine(ActiveItemCooldown());
    }

    private void OnDisable()
    {
        Item.OnRetrieved -= OnItemAdd;
        Hero.OnBenedictionMaxUpgrade -= OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxUpgrade -= OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxDrawback -= OnSpecialAbilityRemove;
        Hero.OnBenedictionMaxDrawback -= OnSpecialAbilityRemove;
        IActiveItem.OnActiveItemCooldownStarted -= () => StartCoroutine(ActiveItemCooldown());
    }

    private void OnItemAdd(ItemEffect itemAdd)
    {
        if (itemAdd is IPassiveItem)
        {
            GameObject frame = CreateFrame(itemPassiveTransform);
            SetFrameItemData(frame, itemAdd);

            if (itemPassiveTransform.childCount > maxItemDisplay)
                DestroyImmediate(itemPassiveTransform.GetChild(0).gameObject);
        }
        else if (itemAdd is IActiveItem)
        {
            activeFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
            SetFrameItemData(activeFrame, itemAdd);
        }
    }

    private void OnSpecialAbilityAdd(ISpecialAbility ability)
    {
        if(ability as DamnationVeil != null)
        {
            specialAbilityFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
            specialAbilityFrame.GetComponentInChildren<RawImage>().texture = damnationVeilIcon;
        }
        else if (ability as DivineShield != null)
        {
            specialAbilityFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
            specialAbilityFrame.GetComponentInChildren<RawImage>().texture = divineShieldIcon;
        }
    }

    private void OnSpecialAbilityRemove()
    {
        specialAbilityFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(false);
    }

    private GameObject CreateFrame(Transform t)
    {
        return Instantiate(framePf, t);
    }

    private void SetFrameItemData(GameObject frame, ItemEffect itemEffect)
    {
        ItemData data = database.GetItem(itemEffect.Name);
        frame.GetComponentInChildren<RawImage>(true).texture = data.icon;
        frame.GetComponent<Image>().color = database.GetItemRarityColor(itemEffect.Name);
    }

    private IEnumerator ActiveItemCooldown()
    {
        float cooldown = 0.0f;
        TMP_Text cooldownTextMesh = activeFrame.GetComponentInChildren<TMP_Text>(true);
        cooldownTextMesh.gameObject.SetActive(true);

        while (cooldown < hero.Inventory.ActiveItem.Cooldown)
        {
            cooldown = Mathf.Max((hero.Inventory.ActiveItem as ItemEffect).CurrentEnergy, 0.0f);
            cooldownTextMesh.text = (Mathf.RoundToInt(hero.Inventory.ActiveItem.Cooldown) - Mathf.RoundToInt(cooldown)).ToString();
            yield return null;
        }

        cooldownTextMesh.gameObject.SetActive(false);
    }
}

static class ItemsExtensions
{
    static public IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
    {
        while (N > source.Count())
        {
            N--;
        }

        //Debug.Log(source.Count() - N);
        return source.Skip(Mathf.Max(0, source.Count() - N));
    }
}
