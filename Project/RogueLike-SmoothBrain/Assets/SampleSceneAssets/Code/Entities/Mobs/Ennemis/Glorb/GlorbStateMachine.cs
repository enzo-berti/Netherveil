// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using UnityEngine;
using StateMachine; // include all script about stateMachine
using System.Collections.Generic;

public class GlorbStateMachine : MonoBehaviour, IGlorb
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }

    public List<Status> StatusToApply => statusToApply;
    [SerializeField] CapsuleCollider shockwaveCollider;
    VFXStopper vfxStopper;
    bool cooldownSpeAttack = false;
    float specialAttackTimer = 0f;
    readonly float SPECIAL_ATTACK_TIMER = 2.2f;

    bool cooldownBasicAttack = false;
    float basicAttackTimer = 0f;
    readonly float BASIC_ATTACK_TIMER = 0.75f;
    bool isDying = false;
    Hero player;
    Animator animator;

    public BaseState<GlorbStateMachine> currentState;
    private StateFactory<GlorbStateMachine> factory;

    void Start()
    {
        factory = new StateFactory<GlorbStateMachine>(this);
        // Set currentState here !
    }

    void Update()
    {
        currentState.Update();
    }
}
