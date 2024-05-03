using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour
{
    private Hero hero;
    private int maxItemDisplay = 5;
    [SerializeField] private KeybindingsIcons iconsList;
    [SerializeField] private GameObject framePf;
    [SerializeField] private ItemDatabase database;
    [SerializeField] private Transform itemPassiveTransform;

    [SerializeField] private GameObject activeFrame;
    [SerializeField] private GameObject specialAbilityFrame;

    [SerializeField] private Sprite backDamnation;
    [SerializeField] private Sprite backDivine;
    [SerializeField] private Sprite[] rarityBackItemSprite;
    [SerializeField] private Sprite[] backItemActiveNormal;
    [SerializeField] private Sprite[] backItemActiveCooldown;

    [SerializeField] private Texture damnationTexture;
    [SerializeField] private Texture divineTexture;

    [SerializeField] private TMP_Text cooldownSpecialAbilityTextMesh;
    [SerializeField] private TMP_Text cooldownActiveTextMesh;
    [SerializeField] private Image keyActiveBack;
    [SerializeField] private Image keyAbilityBack;
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
        IActiveItem.OnActiveItemCooldownStarted += ActiveItemCooldown;
        ISpecialAbility.OnSpecialAbilityActivated += SpecialAbilityCooldown;
        DeviceManager.OnChangedToKB += UpdateKeyboardBiding;
        DeviceManager.OnChangedToGamepad += UpdateGamepadBiding;
        PauseMenu.OnUnpause += UpdateBinding;
    }

    private void OnDisable()
    {
        Item.OnRetrieved -= OnItemAdd;
        Hero.OnBenedictionMaxUpgrade -= OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxUpgrade -= OnSpecialAbilityAdd;
        Hero.OnCorruptionMaxDrawback -= OnSpecialAbilityRemove;
        Hero.OnBenedictionMaxDrawback -= OnSpecialAbilityRemove;
        IActiveItem.OnActiveItemCooldownStarted -= ActiveItemCooldown;
        ISpecialAbility.OnSpecialAbilityActivated -= SpecialAbilityCooldown;
        DeviceManager.OnChangedToKB -= UpdateKeyboardBiding;
        DeviceManager.OnChangedToGamepad -= UpdateGamepadBiding;
        PauseMenu.OnUnpause -= UpdateBinding;
    }

    private void UpdateBinding()
    {
        if(DeviceManager.Instance.IsPlayingKB())
        {
            UpdateKeyboardBiding();
        }
        else
        {
            UpdateGamepadBiding();
        }
    }

    private void UpdateKeyboardBiding()
    {
        string keyActive = keyboardActive.action.bindings.First().path.Split("/").Last();
        string keyAbility = keyboardAbility.action.bindings.First().path.Split("/").Last();

        keyActiveTextMesh.gameObject.SetActive(true);
        keyAbilityTextMesh.gameObject.SetActive(true);

        keyActiveTextMesh.text = keyActive.ToUpper();
        keyAbilityTextMesh.text = keyAbility.ToUpper();

        keyActiveBack.sprite = iconsList.kb.GetSprite(keyActive);
        keyAbilityBack.sprite = iconsList.kb.GetSprite(keyAbility);
    }

    private void UpdateGamepadBiding()
    {
        string keyActive = gamepadActive.action.bindings.First().path.Split("/").Last();
        string keyAbility = gamepadAbility.action.bindings.First().path.Split("/").Last();

        keyActiveTextMesh.gameObject.SetActive(false);
        keyAbilityTextMesh.gameObject.SetActive(false);

        keyActiveTextMesh.text = keyActive.ToUpper();
        keyAbilityTextMesh.text = keyAbility.ToUpper();

        if(DeviceManager.Instance.CurrentDevice is DualShockGamepad)
        {
            keyActiveBack.sprite = iconsList.ps4.GetSprite(keyActive);
            keyAbilityBack.sprite = iconsList.ps4.GetSprite(keyAbility);
        }
        else
        {
            keyActiveBack.sprite = iconsList.xbox.GetSprite(keyActive);
            keyAbilityBack.sprite = iconsList.xbox.GetSprite(keyAbility);
        }
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
        Image image = specialAbilityFrame.GetComponent<Image>();
        RawImage rawImage = specialAbilityFrame.GetComponentInChildren<RawImage>(true);
        rawImage.gameObject.SetActive(true);

        if (ability as DamnationVeil != null)
        {
            image.sprite = backDamnation;
            rawImage.texture = damnationTexture;
        }
        else if (ability as DivineShield != null)
        {
            image.sprite = backDivine;
            rawImage.texture = divineTexture;
        }
    }

    private void OnSpecialAbilityRemove()
    {
        Image image = specialAbilityFrame.GetComponent<Image>();
        RawImage rawImage = specialAbilityFrame.GetComponentInChildren<RawImage>(true);

        image.sprite = rarityBackItemSprite.First();
        rawImage.gameObject.SetActive(false);
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

    private void ActiveItemCooldown(ItemEffect itemEffect)
    {
        StartCoroutine(ActiveItemCooldownCoroutine(itemEffect));
    }

    private void SpecialAbilityCooldown()
    {
        StartCoroutine(SpecialAbilityCooldownCoroutine());
    }

    private IEnumerator ActiveItemCooldownCoroutine(ItemEffect itemEffect)
    {
        float cooldown = 0.0f;

        SetFrameItemData(activeFrame, itemEffect, backItemActiveCooldown);
        cooldownActiveTextMesh.transform.parent.gameObject.SetActive(true);
        Image filler = cooldownActiveTextMesh.transform.parent.GetComponent<Image>();

        while (cooldown < hero.Inventory.ActiveItem.Cooldown)
        {
            filler.fillAmount = (hero.Inventory.ActiveItem.Cooldown - cooldown) / hero.Inventory.ActiveItem.Cooldown;

            cooldown = Mathf.Max((hero.Inventory.ActiveItem as ItemEffect).CurrentEnergy, 0.0f);
            cooldownActiveTextMesh.text = (Mathf.RoundToInt(hero.Inventory.ActiveItem.Cooldown) - Mathf.RoundToInt(cooldown)).ToString();
            yield return null;
        }

        SetFrameItemData(activeFrame, itemEffect, backItemActiveNormal);
        cooldownActiveTextMesh.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator SpecialAbilityCooldownCoroutine()
    {
        float cooldown = 0.0f;
        ISpecialAbility specialAbility = Utilities.Player.GetComponent<PlayerController>().SpecialAbility;

        cooldownSpecialAbilityTextMesh.transform.parent.gameObject.SetActive(true);
        Image filler = cooldownSpecialAbilityTextMesh.transform.parent.GetComponent<Image>();

        while (cooldown < specialAbility.Cooldown)
        {
            filler.fillAmount = (specialAbility.Cooldown - cooldown) / specialAbility.Cooldown;

            cooldown = Mathf.Max(specialAbility.CurrentEnergy, 0.0f);
            cooldownSpecialAbilityTextMesh.text = (Mathf.RoundToInt(specialAbility.Cooldown) - Mathf.RoundToInt(cooldown)).ToString();
            yield return null;
        }

        cooldownSpecialAbilityTextMesh.transform.parent.gameObject.SetActive(false);
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
