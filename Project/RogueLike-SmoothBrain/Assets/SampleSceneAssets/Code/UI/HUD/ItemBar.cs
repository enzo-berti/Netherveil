using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private Sprite backDamnation;
    [SerializeField] private Sprite backDivine;
    [SerializeField] private Sprite[] rarityBackItemSprite;
    [SerializeField] private Sprite[] backItemActiveNormal;
    [SerializeField] private Sprite[] backItemActiveCooldown;
    [SerializeField] private TMP_Text cooldownActiveTextMesh;
    [SerializeField] private TMP_Text keyActiveTextMesh;
    [SerializeField] private TMP_Text keyAbilityTextMesh;

    [Header("Bidings")]
    [SerializeField] private InputActionReference keyboardActive;
    [SerializeField] private InputActionReference keyboardAbility;
    [SerializeField] private InputActionReference gamepadActive;
    [SerializeField] private InputActionReference gamepadAbility;

    private void Start()
    {
        hero = FindObjectOfType<Hero>();

        if (DeviceManager.Instance.IsPlayingKB())
            UpdateKeyboardBiding();
        else
            UpdateGamepadBiding();
    }

    private void OnEnable()
    {
        Item.OnRetrieved += OnItemAdd;
        Hero.OnBenedictionMaxUpgrade += OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxUpgrade += OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxDrawback += OnSpecialAbilityRemove;
        Hero.OnBenedictionMaxDrawback += OnSpecialAbilityRemove;
        IActiveItem.OnActiveItemCooldownStarted += e => StartCoroutine(ActiveItemCooldown(e));
        DeviceManager.OnChangedToKB += UpdateKeyboardBiding;
        DeviceManager.OnChangedToGamepad += UpdateGamepadBiding;
    }

    private void OnDisable()
    {
        Item.OnRetrieved -= OnItemAdd;
        Hero.OnBenedictionMaxUpgrade -= OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxUpgrade -= OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxDrawback -= OnSpecialAbilityRemove;
        Hero.OnBenedictionMaxDrawback -= OnSpecialAbilityRemove;
        IActiveItem.OnActiveItemCooldownStarted -= e => StartCoroutine(ActiveItemCooldown(e));
        DeviceManager.OnChangedToKB -= UpdateKeyboardBiding;
        DeviceManager.OnChangedToGamepad -= UpdateGamepadBiding;
    }

    private void UpdateKeyboardBiding()
    {
        keyActiveTextMesh.text = keyboardActive.action.bindings[0].name;
        keyAbilityTextMesh.text = keyboardAbility.action.bindings[0].name;

        Debug.Log($"t : {keyboardActive.action.id}");
    }

    private void UpdateGamepadBiding()
    {
        keyActiveTextMesh.text = gamepadActive.action.bindings[0].name;
        keyAbilityTextMesh.text = gamepadAbility.action.bindings[0].name;
    }

    private void OnItemAdd(ItemEffect itemAdd)
    {
        if (itemAdd is IPassiveItem)
        {
            GameObject frame = CreateFrame(itemPassiveTransform);
            SetFrameItemData(frame, itemAdd, rarityBackItemSprite);

            if (itemPassiveTransform.childCount > maxItemDisplay)
                DestroyImmediate(itemPassiveTransform.GetChild(0).gameObject);
        }
        else if (itemAdd is IActiveItem)
        {
            activeFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
            SetFrameItemData(activeFrame, itemAdd, backItemActiveNormal);
        }
    }

    private void OnSpecialAbilityAdd(ISpecialAbility ability)
    {
        if(ability as DamnationVeil != null)
        {
            specialAbilityFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
            specialAbilityFrame.GetComponent<Image>().sprite = backDamnation;
        }
        else if (ability as DivineShield != null)
        {
            specialAbilityFrame.GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
            specialAbilityFrame.GetComponent<Image>().sprite = backDivine;
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

    private void SetFrameItemData(GameObject frame, ItemEffect itemEffect, Sprite[] spriteArray)
    {
        ItemData data = database.GetItem(itemEffect.Name);
        frame.GetComponentInChildren<RawImage>(true).texture = data.icon;
        frame.GetComponent<Image>().sprite = spriteArray[(int)data.RarityTier];
    }

    private IEnumerator ActiveItemCooldown(ItemEffect itemEffect)
    {
        float cooldown = 0.0f;

        SetFrameItemData(activeFrame, itemEffect, backItemActiveCooldown);
        cooldownActiveTextMesh.gameObject.SetActive(true);

        while (cooldown < hero.Inventory.ActiveItem.Cooldown)
        {
            cooldown = Mathf.Max((hero.Inventory.ActiveItem as ItemEffect).CurrentEnergy, 0.0f);
            cooldownActiveTextMesh.text = (Mathf.RoundToInt(hero.Inventory.ActiveItem.Cooldown) - Mathf.RoundToInt(cooldown)).ToString();
            yield return null;
        }

        SetFrameItemData(activeFrame, itemEffect, backItemActiveNormal);
        cooldownActiveTextMesh.gameObject.SetActive(false);
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
