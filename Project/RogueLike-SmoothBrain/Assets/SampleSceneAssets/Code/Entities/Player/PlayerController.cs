using FMODUnity;
using Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    Hero hero;
    Animator animator;
    PlayerInput playerInput;
    Transform cameraTransform;
    DialogueTreeRunner dialogueTreeRunner;
    CharacterController characterController;

    [Header("Mechanics")]
    public Spear Spear;
    [SerializeField] GameObject spearThrowWrapper;
    [SerializeField] BoxCollider spearThrowCollider;
    public Collider ChargedAttack;
    public List<NestedList<Collider>> SpearAttacks;
    Plane mouseRaycastPlane;
    readonly float dashCoef = 2.25f;
    public Coroutine SpecialAbilityCoroutine { get; set; } = null;
    public ISpecialAbility SpecialAbility { get; set; } = null;

    public GameObject SpearThrowWrapper { get => spearThrowWrapper; }
    public BoxCollider SpearThrowCollider { get => spearThrowCollider; }

    public bool LaunchUpgradeAnimation { get; set; } = false;
    public bool DoneQuestQTThiStage = false;
    public bool DoneQuestQTApprenticeThiStage = false;

    //rotate values
    public float CurrentTargetAngle { get; set; } = 0f;
    readonly float smoothTime = 0.05f;
    float currentVelocity = 0f;

    //used to auto-redirect on enemies in vision cone when attacking
    const float ATTACK_CONE_ANGLE = 45f;

    //attack values
    public int ComboCount { get; set; } = 0;
    public static readonly int FINISHER_DAMAGES = 10;
    public static readonly int SPEAR_DAMAGES = 5;
    public static readonly int CHARGED_ATTACK_DAMAGES = 20;
    public static readonly int MAX_COMBO_COUNT = 3;
    public static readonly int CHARGED_ATTACK_KNOCKBACK_COEFF = 3;

    [Header("VFXs")]
    [SerializeField] GameObject VFXWrapper;
    public List<VisualEffect> SpearAttacksVFX;
    public VisualEffect HitVFX;
    public VisualEffect DashVFX;
    public VisualEffect ChargedAttackVFX;
    public VisualEffect spearLaunchVFX;
    public VisualEffect corruptionUpgradeVFX;
    public VisualEffect benedictionUpgradeVFX;
    public VisualEffect divineShieldVFX;

    [Header("SFXs")]
    public EventReference DashSFX;
    public EventReference HitSFX;
    public EventReference DeadSFX;
    public EventReference ThrowSpearSFX;
    public EventReference RetrieveSpearSFX;
    public EventReference ChargedAttackMaxSFX;
    public EventReference ChargedAttackReleaseSFX;
    public EventReference HealSFX;
    public EventReference[] AttacksSFX;

    private void Awake()
    {
        hero = GetComponent<Hero>();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
        cameraTransform = Camera.main.transform;
        hero.State = (int)Entity.EntityState.MOVE;
        mouseRaycastPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        //divide by 5 because the vfx is based on plane scale size, and -0.2 is to make the arrow perfectly at the spear end pos
        spearLaunchVFX.SetFloat("VFX Length", hero.Stats.GetValue(Stat.ATK_RANGE) / 5f - 0.2f);

        //initialize starting rotation
        OverridePlayerRotation(225f, true);
        RoomUtilities.allEnemiesDeadEvents += LaunchUpgradeAnim;
        RoomUtilities.allChestOpenEvents += LaunchUpgradeAnim;
    }

    private void OnDestroy()
    {
        RoomUtilities.allEnemiesDeadEvents -= LaunchUpgradeAnim;
    }

    private void Update()
    {
        UpdateAnimator();

        if (CanUpdatePhysic())
        {
            ApplyGravity();
            Rotate();
            Move();
            DashMove();
        }

        //if player has fallen out of map security
        if (transform.position.y < -100f)
        {
            FindObjectOfType<LevelLoader>().LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void UpdateAnimator()
    {
        //used so that you don't see the character running while in transition between the normal attack and the charged attack casting
        float magnitudeCoef = 10;
        if (playerInput.LaunchedChargedAttack || dialogueTreeRunner.IsStarted)
        {
            magnitudeCoef = 0f;
        }

        animator.SetFloat("Speed", playerInput.Direction.magnitude * magnitudeCoef, 0.1f, Time.deltaTime);
        animator.SetInteger("ComboCount", ComboCount);
    }

    #region BasicMovements

    private void Rotate()
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, CurrentTargetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void ApplyGravity()
    {
        if (!CanApplyGravity())
            return;

        characterController.SimpleMove(Vector3.zero);
    }

    private void Move()
    {
        if (!CanMove())
            return;

        CurrentTargetAngle = Mathf.Atan2(playerInput.Direction.x, playerInput.Direction.y) * Mathf.Rad2Deg + cameraTransform.rotation.eulerAngles.y;
        characterController.Move(hero.Stats.GetValue(Stat.SPEED) * Time.deltaTime * playerInput.Direction.ToCameraOrientedVec3().normalized);
    }

    private void DashMove()
    {
        if (hero.State != (int)Hero.PlayerState.DASH)
            return;

        characterController.Move(dashCoef * hero.Stats.GetValue(Stat.SPEED) * Time.deltaTime * playerInput.DashDir);
    }

    #endregion

    #region Attacks&Orientation

    /// <summary>
    ///  Check collision with attack colliders and inflict damages.
    /// </summary>
    /// <param name="colliders"></param>
    /// <param name="debugMode"></param>
    public void AttackCollide(List<Collider> colliders, bool debugMode = true)
    {
        RotatePlayerToDeviceAndMargin();

        List<Collider> alreadyAttacked = new();
        bool applyVibrations = true;
        foreach (Collider collider in colliders)
        {
            ApplyCollide(collider, alreadyAttacked, ref applyVibrations, debugMode);
        }
    }

    /// <summary>
    /// Check collision with attack collider and inflict damages.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="debugMode"></param>
    public void AttackCollide(Collider collider, bool debugMode = true)
    {
        RotatePlayerToDeviceAndMargin();
        List<Collider> alreadyAttacked = new();
        bool applyVibrations = true;
        ApplyCollide(collider, alreadyAttacked, ref applyVibrations, debugMode);
    }

    private void ApplyCollide(Collider collider, List<Collider> alreadyAttacked, ref bool applyVibrations, bool debugMode = true)
    {
        if (debugMode)
        {
            collider.gameObject.SetActive(true);
        }

        //used so that it isn't cast from his feet to ensure that there is no ray fail by colliding with spear or ground
        Vector3 rayOffset = Vector3.up / 2;

        Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(collider, transform.position + rayOffset, "Enemy", LayerMask.GetMask("Map"));

        if (tab.Length > 0)
        {
            foreach (Collider col in tab)
            {
                if (col.gameObject.GetComponent<IDamageable>() != null && !alreadyAttacked.Contains(col))
                {
                    if (applyVibrations && !playerInput.LaunchedChargedAttack)
                    {
                        DeviceManager.Instance.ApplyVibrations(0.1f, 0.1f, 0.15f);
                        applyVibrations = false;
                    }
                    alreadyAttacked.Add(col);
                    hero.Attack(col.gameObject.GetComponent<IDamageable>());
                }
            }
        }
    }

    /// <summary>
    /// rotate the player to mouse's direction if playing KB/mouse
    /// or to joystick direction if using gamepad
    /// and orients automatically the player to an enemy if in the attack cone
    /// </summary>
    private void RotatePlayerToDeviceAndMargin()
    {

        if (DeviceManager.Instance.IsPlayingKB())
        {
            MouseOrientation();
        }
        else
        {
            JoystickOrientation();
        }
        OrientationErrorMargin(hero.Stats.GetValue(Stat.ATK_RANGE));
    }

    /// <summary>
    /// Rotates the player to face the position of the mouse in world space.
    /// </summary>
    public void MouseOrientation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (mouseRaycastPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            float angle = transform.AngleOffsetToFaceTarget(new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z));
            if (angle != float.MaxValue)
            {
                OffsetPlayerRotation(angle, true);
            }
        }
    }

    /// <summary>
    /// Rotates the player based on joystick direction.
    /// </summary>
    public void JoystickOrientation()
    {
        if (playerInput.Direction != Vector2.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(playerInput.Direction.x, 0f, playerInput.Direction.y));
            rotation *= Camera.main.transform.rotation;
            float rotationY = rotation.eulerAngles.y;
            OverridePlayerRotation(rotationY, true);
        }
    }

    /// <summary>
    /// Will automatically redirect the player to face the closest enemy in his vision cone.
    /// </summary>
    /// <param name="visionConeRange"></param>
    public void OrientationErrorMargin(float visionConeRange)
    {
        Transform targetTransform = PhysicsExtensions.OverlapVisionCone(transform.position, ATTACK_CONE_ANGLE, visionConeRange, transform.forward, LayerMask.GetMask("Entity"))
        .Where(x => !x.CompareTag("Player") && x.GetComponent<Transform>() != null)
        .Select(x => x.GetComponent<Transform>())
        .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
        .FirstOrDefault();

        if (targetTransform != null)
        {
            float angle = transform.AngleOffsetToFaceTarget(targetTransform.position, ATTACK_CONE_ANGLE);
            if (angle != float.MaxValue)
            {
                OffsetPlayerRotation(angle, true);
            }
        }
    }
    #endregion

    #region Conditions
    private bool CanMove()
    {
        return hero.State == (int)Entity.EntityState.MOVE && playerInput.Direction != Vector2.zero;
    }

    private bool CanApplyGravity()
    {
        return hero.State != (int)Entity.EntityState.DEAD && hero.State != (int)Hero.PlayerState.DASH;
    }

    private bool CanUpdatePhysic()
    {
        return hero.State != (int)Hero.PlayerState.KNOCKBACK && hero.State != (int)Hero.PlayerState.UPGRADING_STATS
            && characterController != null && characterController.enabled && !dialogueTreeRunner.IsStarted;
    }

    #endregion

    #region Miscellaneous

    private void LaunchUpgradeAnim()
    {
        if (!LaunchUpgradeAnimation)
            return;

        playerInput.DisableGameplayInputs();
        animator.ResetTrigger("UpgradingStats");
        animator.SetTrigger("UpgradingStats");
        hero.State = (int)Hero.PlayerState.UPGRADING_STATS;
        LaunchUpgradeAnimation = false;
        if(hero.Stats.GetValue(Stat.CORRUPTION) > 0)
        {
            corruptionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
        }
        else if (hero.Stats.GetValue(Stat.CORRUPTION) < 0)
        {
            benedictionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
        }
    }

    public void OffsetPlayerRotation(float angleOffset, bool isImmediate = false)
    {
        if (isImmediate)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y += angleOffset;
            transform.eulerAngles = eulerAngles;
            CurrentTargetAngle = transform.eulerAngles.y;
        }
        else
        {
            CurrentTargetAngle += angleOffset;
        }
    }

    public void OverridePlayerRotation(float newAngle, bool isImmediate = false)
    {
        if (isImmediate)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = newAngle;
            transform.eulerAngles = eulerAngles;
        }
        CurrentTargetAngle = newAngle;
    }

    public void PlayVFX(VisualEffect VFX)
    {
        VFXWrapper.transform.SetPositionAndRotation(transform.position, transform.rotation);
        VFX.Play();
    }

    public void ResetValues()
    {
        ComboCount = 0;
        ChargedAttack.gameObject.SetActive(false);

        foreach (NestedList<Collider> colliders in SpearAttacks)
        {
            foreach (Collider collider in colliders.data)
            {
                collider.gameObject.SetActive(false);
            }
        }

        playerInput.ResetValuesInput();
    }

    #endregion
}