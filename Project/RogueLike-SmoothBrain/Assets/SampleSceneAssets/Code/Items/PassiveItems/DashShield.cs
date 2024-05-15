using UnityEngine; 
 
public class DashShield : ItemEffect , IPassiveItem 
{
    public void OnRetrieved() 
    {
        Utilities.PlayerInput.OnStartDash += ApplyShield;
        Utilities.PlayerInput.OnEndDash += RemoveShield;
        Utilities.PlayerInput.OnEndDashAttack += RemoveShield;
    } 
 
    public void OnRemove() 
    {
        Utilities.PlayerInput.OnStartDash -= ApplyShield;
        Utilities.PlayerInput.OnEndDash -= RemoveShield;
        Utilities.PlayerInput.OnEndDashAttack -= RemoveShield;
    } 
 
    private void ApplyShield()
    {
        Utilities.Hero.IsInvincibleCount++;
        Utilities.PlayerController.DashShieldVFX.Reinit();
        Utilities.PlayerController.DashShieldVFX.Play();
        AudioManager.Instance.PlaySound(AudioManager.Instance.DashShieldSFX);
    }

    private void RemoveShield(Vector3 playerPos)
    {
        Utilities.Hero.IsInvincibleCount--;
        Utilities.PlayerController.DashShieldVFX.Stop();
    }
} 
