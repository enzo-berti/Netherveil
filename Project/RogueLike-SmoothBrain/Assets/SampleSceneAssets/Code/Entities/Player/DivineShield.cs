using System.Collections;
using UnityEngine;

public class DivineShield : ISpecialAbility
{
    public float Cooldown { get; set; } = 30f;
    public float CurrentEnergy { get; set; } = 0f;

    private readonly float duration = 3f;
    private float currentTime = 0f;

    public DivineShield() 
    {
        CurrentEnergy = Cooldown;
    }

    public void Activate()
    {
        ISpecialAbility.OnSpecialAbilityActivated?.Invoke();
        PlayerController playerController = Utilities.Player.GetComponent<PlayerController>();
        playerController.DivineShieldVFX.SetFloat("Duration", duration);
        playerController.DivineShieldVFX.Play();
        playerController.SpecialAbilityCoroutine = playerController.StartCoroutine(DisableDivineShield());
        Utilities.Hero.IsInvincibleCount++;
    }

    IEnumerator DisableDivineShield()
    {
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        Utilities.Player.GetComponent<PlayerController>().SpecialAbilityCoroutine = null;
        Utilities.Player.GetComponent<PlayerController>().DivineShieldVFX.Stop();
        currentTime = 0f;
        Utilities.Hero.IsInvincibleCount--;
        yield break;
    }
}
