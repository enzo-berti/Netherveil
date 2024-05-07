// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using StateMachine; // include all script about stateMachine
using UnityEngine;

public class GraftedStateMachine : Mobs/*, IGrafted*/
{
    [HideInInspector]
    public BaseState<GraftedStateMachine> currentState;
    private StateFactory<GraftedStateMachine> factory;

    //    private IAttacker.AttackDelegate onAttack;
    //    private IAttacker.HitDelegate onHit;
    //    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    //    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }

    //    public List<Status> StatusToApply => statusToApply;

    //    float deathTimer = 0;

    //    // anim hash
    //    int dyingHash;
    //    int thrustHash;
    //    int dashHash;
    //    int throwingHash;
    //    int retrievingHash;
    //    int fallHash;
    //    Coroutine tripleThrustCoroutine = null;

    //    enum Attacks
    //    {
    //        THRUST,
    //        DASH,
    //        AOE,
    //        RANGE,
    //        NONE
    //    }

    //    enum AttackState
    //    {
    //        CHARGING,
    //        ATTACKING,
    //        RECOVERING,
    //        IDLE
    //    }

    //    [System.Serializable]
    //    private class GraftedSounds
    //    {
    //        public Sound deathSound;
    //        public Sound hitSound;
    //        public Sound projectileLaunchedSound;
    //        public Sound projectileHitMapSound;
    //        public Sound projectileHitPlayerSound;
    //        public Sound thrustSound;
    //        public Sound introSound;
    //        public Sound retrievingProjectileSound;
    //        public Sound spinAttackSound;
    //        public Sound stretchSound;
    //        public Sound weaponOutSound;
    //        public Sound weaponInSound;
    //        public Sound walkingSound;
    //        public Sound music;

    //        public void StopAllSounds()
    //        {
    //            deathSound.Stop();
    //            deathSound.Stop();
    //            hitSound.Stop();
    //            projectileLaunchedSound.Stop();
    //            projectileHitMapSound.Stop();
    //            projectileHitPlayerSound.Stop();
    //            thrustSound.Stop();
    //            introSound.Stop();
    //            retrievingProjectileSound.Stop();
    //            spinAttackSound.Stop();
    //            stretchSound.Stop();
    //            weaponOutSound.Stop();
    //            weaponInSound.Stop();
    //            walkingSound.Stop();
    //            music.Stop();
    //        }
    //    }

    //    GameObject gameMusic;

    //    [Header("Sounds")]
    //    [SerializeField] private GraftedSounds bossSounds;

    //    Attacks currentAttack = Attacks.NONE;
    //    Attacks lastAttack = Attacks.NONE;
    //    AttackState attackState = AttackState.IDLE;
    //    float attackCooldown = 0;
    //    bool hasProjectile = true;
    //    Hero player = null;
    //    bool playerHit = false;
    //    float height;

    //    [SerializeField] float thrustCharge = 1f;
    //    float thrustChargeTimer;
    //    [Header("Thrust")]
    //    [SerializeField] float thrustDuration = 1f;
    //    float thrustDurationTimer;
    //    int thrustCounter = 0;

    //    [SerializeField] float AOEDuration;
    //    [SerializeField] float dashSpeed = 5f;
    //    float dashChargeTimer = 0f;
    //    float travelledDistance = 0f;
    //    [Header("Dash")]
    //    [SerializeField] float dashRange;
    //    float AOETimer = 0f;
    //    bool triggerAOE = false;

    //    [Header("Range")]
    //    [SerializeField] GameObject projectilePrefab;
    //    GraftedProjectile projectile;
    //    float throwingTimer = 0f;
    //    float rangeSecurity = 0f;

    //    [Header("Boss Attack Hitboxes")]
    //    [SerializeField] List<NestedList<Collider>> attacks;

    //    [SerializeField, Range(0f, 360f)] float visionAngle = 360f;
    //    [SerializeField] float rotationSpeed = 5f;

    //    [Header("VFXs")]
    //    [SerializeField] VisualEffect dashVFX;
    //    [SerializeField] VisualEffect tripleThrustVFX;
    //    bool dashVFXPlayed = false;

    //    CameraUtilities cameraUtilities;

    //    // DEBUG
    //    protected override void Awake()
    //    {
    //        base.Awake();
    //        gameMusic = GameObject.FindGameObjectWithTag("GameMusic");
    //    }

    //    protected override void OnEnable()
    //    {
    //        base.OnEnable();
    //        // jouer l'anim de début de combat

    //        // mettre la cam entre le joueur et le boss

    //        //StartCoroutine(Brain());
    //        if (gameMusic != null)
    //        {
    //            gameMusic.SetActive(false);
    //        }
    //        bossSounds.introSound.Play(transform.position);
    //        bossSounds.music.Play();
    //    }

    //    protected override void OnDisable()
    //    {
    //        base.OnDisable();
    //        if (gameMusic != null)
    //        {
    //            gameMusic.SetActive(true);
    //        }
    //        bossSounds.introSound.Stop();
    //        bossSounds.music.Stop();

    //        bossSounds.StopAllSounds();

    //        StopAllCoroutines();
    //    }

    //    protected override void Start()
    //    {
    //        base.Start();

    //        factory = new StateFactory<GraftedStateMachine>(this);
    //        currentState = factory.GetState<GraftedTriggeredState>();

    //        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();
    //    }

    //    protected override void Update()
    //    {
    //        base.Update();
    //        currentState.Update();
    //    }

    //    #region EDITOR
    //#if UNITY_EDITOR
    //    private void OnDrawGizmos()
    //    {
    //        //if (!Selection.Contains(gameObject))
    //        //    return;

    //        DisplayInfos();
    //    }

    //    protected override void DisplayInfos()
    //    {
    //        Handles.Label(
    //        transform.position + transform.up,
    //        stats.GetEntityName() +
    //        "\n - Health : " + stats.GetValue(Stat.HP) +
    //        "\n - Speed : " + stats.GetValue(Stat.SPEED) +
    //        "\n - State : " + currentState?.ToString(),
    //        new GUIStyle()
    //        {
    //            alignment = TextAnchor.MiddleLeft,
    //            normal = new GUIStyleState()
    //            {
    //                textColor = Color.black
    //            }
    //        });
    //    }
    //#endif
    //    #endregion
}
