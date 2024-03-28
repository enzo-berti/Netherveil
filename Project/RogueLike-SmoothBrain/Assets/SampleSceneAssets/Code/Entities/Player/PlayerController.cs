using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    Transform cameraTransform;
    [Header("Mechanics")]
    readonly float dashCoef = 3f;

    public Plane PlaneOfDoom { get; private set; }
    public List<NestedList<Collider>> spearAttacks;
    public List<Collider> chargedAttack;

    CharacterController characterController;
    readonly float smoothTime = 0.05f;
    float currentVelocity = 0f;
    public float CurrentTargetAngle { get; set; } = 0f;
    public Vector3 DashDir { get; set; } = Vector3.zero;
    public int ComboCount { get; set; } = 0;
    public readonly int MAX_COMBO_COUNT = 3;

    [HideInInspector] public Hero hero;
    PlayerInput playerInput;
    Animator animator;

    //used for the error margin for attacks to auto-redirect on enemies in vision cone
    public const float VISION_CONE_ANGLE = 45f;
    public const float VISION_CONE_RANGE = 8f;

    //attack damages
    public readonly int FINISHER_DAMAGES = 10;
    public readonly int CHARGED_ATTACK_DAMAGES = 20;

    [Header("VFXs")]
    [SerializeField] GameObject VFXWrapper;
    public List<VisualEffect> spearAttacksVFX;
    public VisualEffect hitVFX;
    public VisualEffect dashVFX;
    public VisualEffect chargedAttackVFX;

    [Header("SFXs")]
    public EventReference dashSFX;
    public EventReference hitSFX;
    public EventReference deadSFX;
    public EventReference throwSpearSFX;
    public EventReference retrieveSpearSFX;
    public EventReference chargedAttackMaxSFX;
    public EventReference chargedAttackReleaseSFX;
    public EventReference[] attacksSFX;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        hero = GetComponent<Hero>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
        cameraTransform = Camera.main.transform;
        hero.State = (int)Entity.EntityState.MOVE;
        PlaneOfDoom = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        //initialize starting rotation
        OverridePlayerRotation(225f, true);
    }

    void Update()
    {
        UpdateAnimator();

        if (hero.State != (int)Hero.PlayerState.KNOCKBACK)
        {
            ApplyGravity();
            Rotate();
            Move();
            DashMove();
        }
    }

    private void UpdateAnimator()
    {
        //used so that you don't see the character running while in transition between the normal attack and the charged attack casting
        float magnitudeCoef = 10;
        if (playerInput.LaunchedChargedAttack)
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
        if (hero.State != (int)Entity.EntityState.DEAD && hero.State != (int)Hero.PlayerState.DASH)
        {
            characterController.SimpleMove(Vector3.zero);
        }
    }

    void Move()
    {
        if (hero.State == (int)Entity.EntityState.MOVE && (playerInput.Direction.x != 0f || playerInput.Direction.y != 0f))
        {
            CurrentTargetAngle = Mathf.Atan2(playerInput.Direction.x, playerInput.Direction.y) * Mathf.Rad2Deg + cameraTransform.rotation.eulerAngles.y;
            ModifyCamVectors(out Vector3 camRight, out Vector3 camForward);
            characterController.Move(hero.Stats.GetValue(Stat.SPEED) * Time.deltaTime * (camForward * playerInput.Direction.y + camRight * playerInput.Direction.x).normalized);
        }
    }

    void DashMove()
    {
        if (hero.State == (int)Hero.PlayerState.DASH)
        {
            characterController.Move(dashCoef * hero.Stats.GetValue(Stat.SPEED) * Time.deltaTime * DashDir);
        }
    }

    #endregion

    #region Attacks&Orientation

    public void AttackCollide(List<Collider> colliders, bool debugMode = true)
    {
        if (debugMode)
        {
            foreach (Collider collider in colliders)
            {
                collider.gameObject.SetActive(true);
            }
        }

        //rotate the player to mouse's direction if playing KB/mouse
        if (DeviceManager.Instance.IsPlayingKB())
        {
            MouseOrientation();
        }
        else
        {
            JoystickOrientation();
        }
        OrientationErrorMargin();

        //used so that it isn't cast from his feet to ensure that there is no ray fail by colliding with spear or ground
        Vector3 rayOffset = Vector3.up / 2;

        //used to apply vibrations only one time
        bool applyVibrations = true;
        List<Collider> alreadyAttacked = new List<Collider>();
        foreach (Collider spearCollider in colliders)
        {
            Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(spearCollider, transform.position + rayOffset, "Enemy", LayerMask.GetMask("Map"));

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
    }

    //orients the player to face the position of the mouse
    public void MouseOrientation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (PlaneOfDoom.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            float angle = transform.AngleOffsetToFaceTarget(new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z));
            if (angle != float.MaxValue)
            {
                OffsetPlayerRotation(angle, true);
            }
        }
    }

    public void JoystickOrientation()
    {
        if(playerInput.Direction != Vector2.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(playerInput.Direction.x, 0f, playerInput.Direction.y));
            rotation *= Camera.main.transform.rotation;
            float rotationY = rotation.eulerAngles.y;
            OverridePlayerRotation(rotationY, true);
        }
    }

    //will automatically redirect the player to face the closest enemy in his vision cone
    public void OrientationErrorMargin(float visionConeRange = VISION_CONE_RANGE)
    {
        Transform targetTransform = PhysicsExtensions.OverlapVisionCone(transform.position, VISION_CONE_ANGLE, visionConeRange, transform.forward, LayerMask.GetMask("Entity"))
        .Where(x => !x.CompareTag("Player") && x.GetComponent<Transform>() != null)
        .Select(x => x.GetComponent<Transform>())
        .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
        .FirstOrDefault();

        if (targetTransform != null)
        {
            float angle = transform.AngleOffsetToFaceTarget(targetTransform.position, VISION_CONE_ANGLE);
            if (angle != float.MaxValue)
            {
                OffsetPlayerRotation(angle, true);
            }
        }
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

    /// <summary>
    /// Used to get the directions of camera without the y axis so that the player doesnt move on this axis and renormalize the vectors because of that modification
    /// </summary>
    /// <param name="camRight"></param>
    /// <param name="camForward"></param>
    public void ModifyCamVectors(out Vector3 camRight, out Vector3 camForward)
    {
        camForward = cameraTransform.forward;
        camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }

    public void PlayVFX(VisualEffect VFX)
    {
        VFXWrapper.transform.SetPositionAndRotation(transform.position, transform.rotation);
        VFX.Play();
    }

    public void ResetValues()
    {
        ComboCount = 0;

        foreach (Collider collider in chargedAttack)
        {
            collider.gameObject.SetActive(false);
        }

        foreach (NestedList<Collider> colliders in spearAttacks)
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