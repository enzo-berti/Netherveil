using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    Transform cameraTransform;
    [Range(0f, 20f), SerializeField]
    float dashSpeed;

    public List<NestedList<Collider>> spearAttacks;
    public List<Collider> chargedAttack;

    CharacterController characterController;
    readonly float smoothTime = 0.05f;
    float currentVelocity = 0f;
    public float CurrentTargetAngle { get; set; } = 0f;
    public Vector2 DashDir { get; set; } = Vector2.zero;
    public Vector2 LastDir { get; set; } = Vector2.zero;
    public int ComboCount { get; set; } = 0;
    public readonly int MAX_COMBO_COUNT = 3;

    [HideInInspector] public Hero hero;

    public Vector2 Direction { get; private set; } = Vector2.zero;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        hero = GetComponent<Hero>();
        cameraTransform = Camera.main.transform;
        hero.State = (int)Entity.EntityState.MOVE;

        //initialize starting rotation
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.y = 225f;
        transform.eulerAngles = eulerAngles;
        CurrentTargetAngle = transform.eulerAngles.y;
    }

    void Update()
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, CurrentTargetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Move();
        DashMove();
    }

    void Move()
    {
        if (hero.State == (int)Entity.EntityState.MOVE && (Direction.x != 0f || Direction.y != 0f))
        {
            LastDir = Direction;
            CurrentTargetAngle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg + cameraTransform.rotation.eulerAngles.y;
            ModifyCamVectors(out Vector3 camRight, out Vector3 camForward);

            characterController.Move(hero.Stats.GetValueStat(Stat.SPEED) * Time.deltaTime * (camForward * Direction.y + camRight * Direction.x).normalized);
        }
    }

    void DashMove()
    {
        if (hero.State == (int)Hero.PlayerState.DASH)
        {
            characterController.Move(dashSpeed * Time.deltaTime * transform.forward);
        }
    }

    public void ReadDirection(InputAction.CallbackContext ctx)
    {
        Direction = ctx.ReadValue<Vector2>().normalized;
    }

    public Collider[] CheckAttackCollide(Collider collider, Vector3 rayOrigin, string targetTag, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (collider != null)
        {
            System.Type colliderType = collider.GetType();

            switch (colliderType.Name)
            {
                case nameof(BoxCollider):
                    return (collider as BoxCollider).BoxOverlapWithRayCheck(rayOrigin, targetTag, layerMask, queryTriggerInteraction);
                case nameof(SphereCollider):
                    return (collider as SphereCollider).SphereOverlapWithRayCheck(rayOrigin, targetTag, layerMask, queryTriggerInteraction);
                case nameof(CapsuleCollider):
                    return (collider as CapsuleCollider).CapsuleOverlapWithRayCheck(rayOrigin, targetTag, layerMask, queryTriggerInteraction);
                default:
                    Debug.LogWarning("Invalid Collider type, can't check the collision.");
                    return new Collider[0];
            }
        }

        Debug.LogWarning("Collider is null.");
        return new Collider[0];
    }

    /// <summary>
    /// Used to get the directions of camera without the y axis so that the player doesnt move on this axis and renormalize the vectors because of that modification
    /// </summary>
    /// <param name="camRight"></param>
    /// <param name="camForward"></param>
    void ModifyCamVectors(out Vector3 camRight, out Vector3 camForward)
    {
        camForward = cameraTransform.forward;
        camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }
}