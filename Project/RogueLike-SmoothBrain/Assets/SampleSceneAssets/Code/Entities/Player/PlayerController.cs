using FMODUnity;
using Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] BoxCollider dashAttackCollider;
    public GameObject ezrealAttackPrefab;
    public Collider ChargedAttack;
    public List<NestedList<Collider>> SpearAttacks;
    Plane mouseRaycastPlane;
    readonly float dashCoef = 2.25f;
    private List<Collider> collidersIgnored = new List<Collider>();
    public Coroutine SpecialAbilityCoroutine { get; set; } = null;
    public ISpecialAbility SpecialAbility { get; set; } = null;

    public GameObject SpearThrowWrapper { get => spearThrowWrapper; }
    public BoxCollider SpearThrowCollider { get => spearThrowCollider; }
    public BoxCollider DashAttackCollider { get => dashAttackCollider; }
    public List<Collider> CollidersIgnored { get => collidersIgnored; }

    public bool DoneQuestQTThiStage { get; set; } = false;
    public bool DoneQuestQTApprenticeThiStage { get; set; } = false;
    public bool ClearedTuto { get; set; } = false;

    //rotate values
    public float CurrentTargetAngle { get; set; } = 0f;
    readonly float smoothTime = 0.05f;
    float currentVelocity = 0f;

    //used to auto-redirect on enemies in vision cone when attacking
    const float ATTACK_CONE_ANGLE = 45f;

    //attack values
    public int ComboCount { get; set; } = 0;
    public readonly int MAX_COMBO_COUNT = 3;

    public int FINISHER_DAMAGES { get; private set; }
    public int BASIC_ATTACK_DAMAGES { get; private set; }
    public int SPEAR_DAMAGES { get; private set; }
    public int CHARGED_ATTACK_DAMAGES { get; private set; }

    public readonly int CHARGED_ATTACK_KNOCKBACK_COEFF = 3;

    //animator hashs
    public int SpeedHash { get; private set; }
    public int IsDeadHash { get; private set; }
    public int IsKnockbackHash { get; private set; }
    public int DashHash { get; private set; }
    public int DashAttackHash { get; private set; }
    public int BasicAttackHash { get; private set; }
    public int ComboCountHash { get; private set; }
    public int SpearThrowingHash { get; private set; }
    public int SpearThrownHash { get; private set; }
    public int ChargedAttackReleaseHash { get; private set; }
    public int ChargedAttackCastingHash { get; private set; }
    public int LaunchBombHash { get; private set; }
    public int CorruptionUpgradeHash { get; private set; }
    public int BenedictionUpgradeHash { get; private set; }
    public int PouringBloodHash { get; private set; }

    [Header("VFXs")]
    [SerializeField] GameObject VFXWrapper;
    [SerializeField] SkinnedMeshRenderer bodyMesh;
    public List<VisualEffect> SpearAttacksVFX;
    public VisualEffect DashAttackVFX;
    public VisualEffect HitVFX;
    public VisualEffect DashVFX;
    public VisualEffect ChargedAttackVFX;
    public VisualEffect SpearLaunchVFX;
    public VisualEffect corruptionUpgradeVFX;
    public VisualEffect benedictionUpgradeVFX;
    public VisualEffect DrawbackVFX;
    public VisualEffect DivineShieldVFX;
    public VisualEffect DamnationVeilVFX;
    public VisualEffect DashShieldVFX;
    public VisualEffect RuneOfSlothVFX;
    public VisualEffect RuneOfPrideVFX;

    public SkinnedMeshRenderer BodyMesh { get => bodyMesh; }

    [Header("SFXs")]
    public EventReference DashSFX;
    public EventReference HitSFX;
    public EventReference DeadSFX;
    public EventReference ThrowSpearSFX;
    public EventReference RetrieveSpearSFX;
    public EventReference ChargedAttackMaxSFX;
    public EventReference ChargedAttackReleaseSFX;
    public EventReference HealSFX;
    public EventReference BenedictionUpgradeSFX;
    public EventReference CorruptionUpgradeSFX;
    public EventReference StepDowngradeSFX;
    public EventReference[] AttacksSFX;

    [Header("Item dependent GOs")]
    [SerializeField] Transform leftHandTransform;
    public Transform LeftHandTransform { get => leftHandTransform; }
    private List<Vector3> ENDPOS = new();
    private List<Color> color = new() { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.white };
    private void Awake()
    {
        hero = GetComponent<Hero>();
        FINISHER_DAMAGES = (int)(hero.Stats.GetValueWithoutCoeff(Stat.ATK) * 2);
        BASIC_ATTACK_DAMAGES = (int)hero.Stats.GetValueWithoutCoeff(Stat.ATK);
        SPEAR_DAMAGES = 0;
        CHARGED_ATTACK_DAMAGES = (int)(hero.Stats.GetValueWithoutCoeff(Stat.ATK) * 9);
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
        SpearLaunchVFX.SetFloat("VFX Length", hero.Stats.GetValue(Stat.ATK_RANGE) / 5f - 0.2f);

        //initialize starting rotation
        OverridePlayerRotation(225f, true);
        MapUtilities.onFinishStage += ResetStageDependentValues;
        MapUtilities.onAllEnemiesDead += UpdateClearTuto;

        SpeedHash = Animator.StringToHash("Speed");
        IsDeadHash = Animator.StringToHash("IsDead");
        IsKnockbackHash = Animator.StringToHash("IsKnockback");
        DashHash = Animator.StringToHash("Dash");
        DashAttackHash = Animator.StringToHash("DashAttack");
        BasicAttackHash = Animator.StringToHash("BasicAttack");
        ComboCountHash = Animator.StringToHash("ComboCount");
        SpearThrowingHash = Animator.StringToHash("SpearThrowing");
        SpearThrownHash = Animator.StringToHash("SpearThrown");
        ChargedAttackReleaseHash = Animator.StringToHash("ChargedAttackRelease");
        ChargedAttackCastingHash = Animator.StringToHash("ChargedAttackCasting");
        LaunchBombHash = Animator.StringToHash("LaunchBomb");
        CorruptionUpgradeHash = Animator.StringToHash("CorruptionUpgrade");
        BenedictionUpgradeHash = Animator.StringToHash("BenedictionUpgrade");
        PouringBloodHash = Animator.StringToHash("PouringBlood");
    }

    private void OnDestroy()
    {
        MapUtilities.onFinishStage -= ResetStageDependentValues;
        MapUtilities.onAllEnemiesDead -= UpdateClearTuto;
        Spear.OnPlacedInHand = null;
        Spear.OnPlacedInWorld = null;
        Spear.OnLatePlacedInWorld = null;

        //reset coroutine manager to avoid any problems when reloading ingame
        CoroutineManager.StopAllCoroutinesInstance();
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
            hero.Death();
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

        animator.SetFloat(SpeedHash, playerInput.Direction.magnitude * magnitudeCoef, 0.1f, Time.deltaTime);
        animator.SetInteger(ComboCountHash, ComboCount);
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

        float distance = dashCoef * hero.Stats.GetValue(Stat.SPEED) * Time.deltaTime;
        //if(Utilities.CharacterController.Raycast(new Ray(this.gameObject.transform.position, playerInput.DashDir), out var hit, distance))
        //{
        //    Debug.Log(hit.collider.name);
        //}
        if (Utilities.CharacterController.detectCollisions)
            //if (Physics.BoxCast()
            //{
            //    Debug.Log(hit.collider.name);
            //}
            characterController.Move(distance * playerInput.DashDir);
    }

    public void RemoveCollisionOnDash(float _dashDuration)
    {
        // Player capsule collider ( character controller ) params
        const float sphereCastSize = 0.5f;
        const float height = 1.9f;

        Vector3 currentEndPos = this.transform.position + dashCoef * hero.Stats.GetValue(Stat.SPEED) * _dashDuration * playerInput.DashDir;
        currentEndPos.y = 0;
        Vector3 baseFinalPos = currentEndPos;

        /*  ---- FOR GIZMOS ---
        ENDPOS.Clear();
        ENDPOS.Add(currentEndPos);
        */

        // Check if there is a wall on the dash's path
        if (Physics.Raycast(transform.position, playerInput.DashDir, out var endHit, (currentEndPos - this.transform.position).magnitude, ~LayerMask.GetMask("AvoidDashCollide")))
        {
            // If it is, replace EndPos close to the hit point on wall
            currentEndPos = endHit.point;
            // Used to align every EndPos on the vector (baseFinalPos, player)
            currentEndPos = Vector3.Project((currentEndPos - baseFinalPos), (this.transform.position - baseFinalPos)) + baseFinalPos;
            currentEndPos.y = 0;

            /* --- FOR GIZMOS ---
            ENDPOS.Add(currentEndPos);*/
        }

        Vector3 capsuleBase = this.transform.position;
        Vector3 capsuleTop = new Vector3(capsuleBase.x, capsuleBase.y + height, capsuleBase.z);
        List<RaycastHit> hits = Physics.CapsuleCastAll(capsuleBase, capsuleTop, sphereCastSize, playerInput.DashDir, (currentEndPos - this.transform.position).magnitude, LayerMask.GetMask("AvoidDashCollide")).ToList();
        List<Collider> ToCollide = new List<Collider>();
        // For each collider on the dash path
        for (int i = hits.Count - 1; i >= 0; i--)
        {
            Collider collider = hits[i].collider;
            capsuleBase = currentEndPos;
            capsuleTop = new Vector3(capsuleBase.x, capsuleBase.y + height, capsuleBase.z);
            Collider[] overlapColliders = Physics.OverlapCapsule(capsuleBase, capsuleTop, sphereCastSize, LayerMask.GetMask("AvoidDashCollide"));
            // Put on List ToCollide every collider in the overlapCapsule from the currentEndPosition
            foreach (var collideOnCurrentEnd in overlapColliders)
            {
                if (!ToCollide.Contains(collideOnCurrentEnd))
                {
                    ToCollide.Add(collideOnCurrentEnd);
                }
            }
            // Then if the current collider is on the overlapColliders, then replace the current EndPos on the current colliders
            if (overlapColliders.Contains(collider))
            {
                currentEndPos = hits[i].point;
                currentEndPos = Vector3.Project(currentEndPos - baseFinalPos, this.transform.position - baseFinalPos) + baseFinalPos;
                currentEndPos.y = 0;
                /* --- FOR GIZMOS ---
                ENDPOS.Add(currentEndPos);
                hits.RemoveAt(i); */
            }
        }

        // Finally, for each colliders in the dash path
        foreach (var hit in hits)
        {
            Collider collider = hit.collider;
            capsuleBase = currentEndPos;
            capsuleTop = new Vector3(capsuleBase.x, capsuleBase.y + 2, capsuleBase.z);

            // If the collider isn't in ToCollide List && not in the last overlapCapsule from the endPos, Ignore its collision until the end of the dash
            if(!ToCollide.Contains(collider) && !Physics.OverlapCapsule(capsuleBase, capsuleTop, sphereCastSize, LayerMask.GetMask("AvoidDashCollide")).Contains(collider))
            {
                Physics.IgnoreCollision(characterController, collider, true);
                collidersIgnored.Add(collider);
            }
            
        }

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

    public void ApplyCollide(Collider collider, List<Collider> alreadyAttacked, ref bool applyVibrations, bool debugMode = true)
    {
        if (debugMode)
        {
            collider.gameObject.SetActive(true);
        }

        //used so that it isn't cast from his feet to ensure that there is no ray fail by colliding with spear or ground
        Vector3 rayOffset = Vector3.up / 2;

        bool corruptionNerfApplied = false;

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
                    if (!corruptionNerfApplied)
                    {
                        //hero.CorruptionNerf(/*col.gameObject.GetComponent<IDamageable>(), hero*/);
                        corruptionNerfApplied = true;
                    }

                    alreadyAttacked.Add(col);
                    hero.Attack(col.gameObject.GetComponent<IDamageable>());
                }
                else if (col.gameObject.GetComponent<IReflectable>() != null && !alreadyAttacked.Contains(col))
                {
                    if (applyVibrations && !playerInput.LaunchedChargedAttack)
                    {
                        DeviceManager.Instance.ApplyVibrations(0.1f, 0.1f, 0.15f);
                        applyVibrations = false;
                    }
                    alreadyAttacked.Add(col);

                    Vector3 direction = col.transform.position - transform.position;
                    direction.y = 0f;
                    direction.Normalize();
                    col.gameObject.GetComponent<IReflectable>().Reflect(direction);
                }
            }
        }
    }

    /// <summary>
    /// rotate the player to mouse's direction if playing KB/mouse
    /// or to joystick direction if using gamepad
    /// and orients automatically the player to an enemy if in the attack cone
    /// </summary>
    public void RotatePlayerToDeviceAndMargin()
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
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (mouseRaycastPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            hitPoint.y = this.transform.position.y;
            hitPoint += (hitPoint - this.transform.position).normalized * 50f;

            float angle = transform.AngleOffsetToFaceTarget(hitPoint);
            angle = System.MathF.Round(angle, 1);
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
        .Where(x => x.CompareTag("Enemy") && x.GetComponent<IReflectable>() == null
        && x.gameObject.TryGetComponent(out Entity entity) && entity.IsInvincibleCount == 0)
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

        ChargedAttackVFX.Stop();
        foreach (VisualEffect effect in SpearAttacksVFX)
        {
            effect.Stop();
        }

        DashAttackVFX.Stop();
        SpearLaunchVFX.Stop();
        VFXWrapper.transform.SetPositionAndRotation(transform.position, transform.rotation);
        VFX.Play();
    }

    public void ResetValues()
    {
        ComboCount = 0;
        ChargedAttack.gameObject.SetActive(false);
        dashAttackCollider.gameObject.SetActive(false);

        foreach (NestedList<Collider> colliders in SpearAttacks)
        {
            foreach (Collider collider in colliders.data)
            {
                collider.gameObject.SetActive(false);
            }
        }

        playerInput.ResetValuesInput();
    }

    public void UpdateVFXWrapperTransform()
    {
        VFXWrapper.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    public void PlayBloodPouringAnim()
    {
        hero.State = (int)Hero.PlayerState.MOTIONLESS;
        animator.ResetTrigger(PouringBloodHash);
        animator.SetTrigger(PouringBloodHash);
    }

    public void PlayLaunchBombAnim()
    {
        hero.State = (int)Hero.PlayerState.MOTIONLESS;
        animator.ResetTrigger(LaunchBombHash);
        animator.SetTrigger(LaunchBombHash);
    }

    private void UpdateClearTuto()
    {
        if (MapUtilities.currentRoomData.Type == RoomType.Tutorial)
        {
            ClearedTuto = true;
        }
    }

    private void ResetStageDependentValues()
    {
        DoneQuestQTThiStage = false;
        DoneQuestQTApprenticeThiStage = false;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;


        //Handles.color = new Color(1, 1, 0.5f, 0.2f);
        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, ATTACK_CONE_ANGLE / 2f, (int)hero.Stats.GetValue(Stat.ATK_RANGE));
        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -ATTACK_CONE_ANGLE / 2f, (int)hero.Stats.GetValue(Stat.ATK_RANGE));

        //Handles.color = Color.white;
        //Handles.DrawWireDisc(transform.position, Vector3.up, (int)hero.Stats.GetValue(Stat.ATK_RANGE));



        int i = 0;
        if (ENDPOS.Count > 0)
        {
            foreach (var test in ENDPOS)
            {
                Gizmos.color = color[i];

                //Gizmos.DrawSphere(test, 0.5f);
                i++;
                i = i == color.Count - 1 ? 0 : i;
                //Gizmos.DrawSphere(test, 0.5f);
                DrawWireCapsule(test, Quaternion.identity, 0.5f, 1.9f, Gizmos.color);
            }
        }

    }
    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
    {
        if (_color != default(Color))
            Handles.color = _color;
        _pos.y += _height / 2;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

        }
    }
#endif
}

