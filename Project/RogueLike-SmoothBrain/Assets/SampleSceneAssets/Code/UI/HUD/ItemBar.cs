using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ItemBar : MonoBehaviour
{
    private Hero hero;
    private int maxItemDisplay = 5;

    [Header("General")]
    [SerializeField] private KeybindingsIcons iconsList;
    [SerializeField] private ItemDatabase database;
    [SerializeField] private Transform itemPassiveTransform;
    [SerializeField] private ItemFrame framePf;

    [Header("Backgrounds")]
    [SerializeField] private Sprite backDamnation;
    [SerializeField] private Sprite backDivine;
    [SerializeField] private Sprite[] rarityBackItemSprite;
    [SerializeField] private Sprite[] backItemActiveNormal;

    [Header("Sprites")]
    [SerializeField] private Sprite damnationSprite;
    [SerializeField] private Sprite divineSprite;

    [Header("Specials frames")]
    [SerializeField] private SpecialItemFrame specialAbilityFrame;
    [SerializeField] private SpecialItemFrame specialItemFrame;

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
        specialItemFrame.SetKey(iconsList.kb.GetSprite(keyActive), keyActive.ToUpper());

        string keyAbility = keyboardAbility.action.bindings.First().path.Split("/").Last();
        specialAbilityFrame.SetKey(iconsList.kb.GetSprite(keyAbility), keyAbility.ToUpper());
    }

    private void UpdateGamepadBiding()
    {
        string keyActive = gamepadActive.action.bindings.First().path.Split("/").Last();
        Sprite activeSprite = DeviceManager.Instance.CurrentDevice is DualShockGamepad ? iconsList.ps4.GetSprite(keyActive) : iconsList.xbox.GetSprite(keyActive);
        specialItemFrame.SetKey(activeSprite, keyActive.ToUpper());

        string keyAbility = gamepadAbility.action.bindings.First().path.Split("/").Last();
        Sprite abilitySprite = DeviceManager.Instance.CurrentDevice is DualShockGamepad ? iconsList.ps4.GetSprite(keyAbility) : iconsList.xbox.GetSprite(keyAbility);
        specialAbilityFrame.SetKey(abilitySprite, keyAbility.ToUpper());
    }

    private void OnItemAdd(ItemEffect itemAdd)
    {
        ItemData data = database.GetItem(itemAdd.Name);
        Sprite item = Sprite.Create((Texture2D)data.icon, new Rect(0.0f, 0.0f, data.icon.width, data.icon.height), new Vector2(0.5f, 0.5f), 100.0f);

        if (itemAdd is IPassiveItem)
        {
            ItemFrame frame = Instantiate(framePf, itemPassiveTransform);
            frame.SetFrame(rarityBackItemSprite[(int)data.RarityTier], item);

            if (itemPassiveTransform.childCount > maxItemDisplay)
                DestroyImmediate(itemPassiveTransform.GetChild(0).gameObject);
        }
        else if (itemAdd is IActiveItem)
        {
            specialItemFrame.SetFrame(backItemActiveNormal[(int)data.RarityTier], item);
        }
    }

    private void OnSpecialAbilityAdd(ISpecialAbility ability)
    {

        if (ability as DamnationVeil != null)
        {
            specialAbilityFrame.SetFrame(backDamnation, damnationSprite);
        }
        else if (ability as DivineShield != null)
        {
            specialAbilityFrame.SetFrame(backDivine, divineSprite);
        }
    }

    private void OnSpecialAbilityRemove()
    {
        specialAbilityFrame.SetFrame(rarityBackItemSprite.First(), null);
    }

    private void ActiveItemCooldown(ItemEffect itemEffect)
    {
        ItemData data = database.GetItem(itemEffect.Name);
        Sprite item = Sprite.Create((Texture2D)data.icon, new Rect(0.0f, 0.0f, data.icon.width, data.icon.height), new Vector2(0.5f, 0.5f), 100.0f);
        specialItemFrame.SetFrame(rarityBackItemSprite[(int)data.RarityTier], item);

        float cooldown = (itemEffect as IActiveItem).Cooldown;

        StartCoroutine(CooldownRoutine(cooldown, specialItemFrame));
    }

    private void SpecialAbilityCooldown()
    {
        float cooldown = Utilities.Player.GetComponent<PlayerController>().SpecialAbility.Cooldown;

        StartCoroutine(CooldownRoutine(cooldown, specialAbilityFrame));
    }

    private IEnumerator CooldownRoutine(float duration, SpecialItemFrame frame)
    {
        float elapsed = 0.0f;

        frame.ToggleCooldown(true);

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            frame.SetCooldown(elapsed, duration);

            yield return null;
        }

        frame.ToggleCooldown(false);
    }
}