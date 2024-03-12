using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity, IDamageable, IAttacker, IBlastable
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB
    }
    Animator animator;
    PlayerInput playerInput;
    PlayerController playerController;
    Inventory inventory = new Inventory();
    public Inventory Inventory { get { return inventory; } }

    public delegate void KillDelegate(IDamageable damageable);
    private KillDelegate onKill;

    public delegate void ChangeRoomDelegate();
    private ChangeRoomDelegate onChangeRoom;

    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public KillDelegate OnKill { get => onKill; set => onKill = value; }
    public ChangeRoomDelegate OnChangeRoom { get => onChangeRoom; set => onChangeRoom = value; }

    private List<Status> statusToApply = new List<Status>();
    public List<Status> StatusToApply => statusToApply;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();

        statusToApply.Add(new Fire(3f));

        if (this is IAttacker attacker)
        {
            attacker.OnHit += attacker.ApplyStatus;
        }
    }


    public void ApplyDamage(int _value, bool hasAnimation = true)
    {
        Stats.DecreaseValue(Stat.HP, _value, false);
        FloatingTextGenerator.CreateDamageText(_value, transform.position);
        if (hasAnimation && (-_value) < 0 && stats.GetValue(Stat.HP) > 0) //just to be sure it really inflicts damages
        {
            State = (int)EntityState.HIT;
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");
            AudioManager.Instance.PlaySound(playerController.playerHit);
        }

        if (stats.GetValue(Stat.HP) <= 0 && State != (int)EntityState.DEAD)
        {
            Death();
            AudioManager.Instance.PlaySound(playerController.playerDead);
        }
    }
    public void Death()
    {
        OnDeath?.Invoke(this.transform.position);
        Destroy(GetComponent<CharacterController>());
        animator.applyRootMotion = true;
        State = (int)EntityState.DEAD;
        animator.ResetTrigger("Death");
        animator.SetTrigger("Death");
    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        if (playerInput.LaunchedChargedAttack)
        {
            damages += (int)(playerController.CHARGED_ATTACK_DAMAGES * playerInput.ChargedAttackCoef);
        }
        else if (playerController.ComboCount == playerController.MAX_COMBO_COUNT -1)
        {
            damages += playerController.FINISHER_DAMAGES;
        }

        damages = (int)(damages * stats.GetValue(Stat.ATK_COEFF));
        damageable.ApplyDamage(damages);

        onHit?.Invoke(damageable);

        if (damageable is IKnockbackable)
        {
            Vector3 force = ((damageable as MonoBehaviour).transform.position - transform.position).normalized;
            (damageable as IKnockbackable).GetKnockback(force * stats.GetValue(Stat.KNOCKBACK_COEFF));
            FloatingTextGenerator.CreateActionText((damageable as MonoBehaviour).transform.position, "Pushed!");
        }
    }
}