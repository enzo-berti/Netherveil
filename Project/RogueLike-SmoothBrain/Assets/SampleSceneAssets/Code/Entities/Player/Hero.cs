using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hero : Entity, IDamageable, IAttacker, IBlastable
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB,
        KNOCKBACK
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
    public static event Action OnTakeDamage; 
    
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public KillDelegate OnKill { get => onKill; set => onKill = value; }
    public ChangeRoomDelegate OnChangeRoom { get => onChangeRoom; set => onChangeRoom = value; }

    public List<Status> StatusToApply => statusToApply;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();

        statusToApply.Add(new Fire(3f));

        if (this is IAttacker attacker)
        {
            attacker.OnHit += attacker.ApplyStatus;
        }
    }


    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        Stats.DecreaseValue(Stat.HP, _value, false);
       
        if ((-_value) < 0 && stats.GetValue(Stat.HP) > 0) //just to be sure it really inflicts damages
        {
           if(hasAnimation)
            {
                if(!playerInput.LaunchedChargedAttack)
                {
                    //animator.ResetTrigger("Hit");
                    //animator.SetTrigger("Hit");
                    playerController.ResetValues(); //possible source de bugs
                    animator.ResetTrigger("ChargedAttackRelease");
                    animator.ResetTrigger("ChargedAttackCharging");
                    animator.ResetTrigger("BasicAttack");
                }
                AudioManager.Instance.PlaySound(playerController.hitSFX);
                FloatingTextGenerator.CreateEffectDamageText(_value, transform.position, Color.red);
                //FloatingTextGenerator.CreateDamageText(_value, transform.position, isCrit);
            }

            OnTakeDamage?.Invoke();
        }

        if (stats.GetValue(Stat.HP) <= 0 && State != (int)EntityState.DEAD)
        {
            Death();
            AudioManager.Instance.PlaySound(playerController.deadSFX);
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
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        //bool isCrit = UnityEngine.Random.Range(0, 101) <= stats.GetValue(Stat.CRIT_RATE);
        //if (isCrit)
        //{
        //    float critDamageCoef = stats.GetValue(Stat.CRIT_DAMAGE)/100;
        //    damages = (int)(damages * critDamageCoef);
        //}

        damages = (int)(damages * stats.GetValue(Stat.ATK_COEFF));
        damageable.ApplyDamage(damages/*, isCrit*/);

        onHit?.Invoke(damageable);

        Knockback knockbackable = (damageable as MonoBehaviour).GetComponent<Knockback>();
        if (knockbackable)
        {
            Vector3 damageablePos = (damageable as MonoBehaviour).transform.position;
            Vector3 force = new Vector3(damageablePos.x - transform.position.x, 0f, damageablePos.z - transform.position.z).normalized;
            knockbackable.GetKnockback(force * stats.GetValue(Stat.KNOCKBACK_COEFF));
            FloatingTextGenerator.CreateActionText((damageable as MonoBehaviour).transform.position, "Pushed!");
        }
    }
}