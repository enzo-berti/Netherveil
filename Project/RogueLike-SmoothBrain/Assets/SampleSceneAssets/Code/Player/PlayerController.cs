using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform cameraTransform;
    [Range(0f, 20f), SerializeField]
    float dashSpeed;

    public Plane PlaneOfDoom { get; private set; }
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

    //used for the error margin for attacks to auto-redirect on enemies in vision cone
    public const float VISION_CONE_ANGLE = 45f;
    public const float VISION_CONE_RANGE = 8f;

    //attack damages
    public readonly int FINISHER_DAMAGES = 10;
    public readonly int CHARGED_ATTACK_DAMAGES = 20;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        hero = GetComponent<Hero>();
        cameraTransform = Camera.main.transform;
        hero.State = (int)Entity.EntityState.MOVE;
        PlaneOfDoom = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        //initialize starting rotation
        OverridePlayerRotation(225f, true);
    }

    void Update()
    {
        //used to apply gravity
        if (hero.State != (int)Entity.EntityState.DEAD)
        {
            characterController.SimpleMove(Vector3.zero);
        }
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

            characterController.Move(hero.Stats.GetValue(Stat.SPEED) * Time.deltaTime * (camForward * Direction.y + camRight * Direction.x).normalized);
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

    public Collider[] CheckAttackCollide(Collider collider, Vector3 rayOrigin, string targetTag, int obstacleLayer = -1, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (collider != null)
        {
            System.Type colliderType = collider.GetType();

            switch (colliderType.Name)
            {
                case nameof(BoxCollider):
                    return (collider as BoxCollider).BoxOverlapWithRayCheck(rayOrigin, targetTag, obstacleLayer, layerMask, queryTriggerInteraction);
                case nameof(SphereCollider):
                    return (collider as SphereCollider).SphereOverlapWithRayCheck(rayOrigin, targetTag, obstacleLayer, layerMask, queryTriggerInteraction);
                case nameof(CapsuleCollider):
                    return (collider as CapsuleCollider).CapsuleOverlapWithRayCheck(rayOrigin, targetTag, obstacleLayer, layerMask, queryTriggerInteraction);
                default:
                    Debug.LogWarning("Invalid Collider type, can't check the collision.");
                    return new Collider[0];
            }
        }

        Debug.LogWarning("Collider is null.");
        return new Collider[0];
    }

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
        if (InputDeviceManager.Instance.IsPlayingKB())
        {
            MouseOrientation();
        }
        OrientationErrorMargin();

        //used so that it isn't cast from his feet to ensure that there is no ray fail by colliding with spear or ground
        Vector3 rayOffset = Vector3.up / 2;

        List<Collider> alreadyAttacked = new List<Collider>();
        foreach (Collider spearCollider in colliders)
        {
            Collider[] tab = CheckAttackCollide(spearCollider, transform.position + rayOffset, "Enemy", LayerMask.GetMask("Map"));
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<IDamageable>() != null && !alreadyAttacked.Contains(col))
                    {
                        //Debug.Log(col.gameObject.name);
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
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z);
            //Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 1000f); // This will draw the ray for 10 seconds

            float angle = transform.AngleOffsetToFaceTarget(new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z));
            if (angle != float.MaxValue)
            {
                OffsetPlayerRotation(angle, true);
            }
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
    void ModifyCamVectors(out Vector3 camRight, out Vector3 camForward)
    {
        camForward = cameraTransform.forward;
        camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Collider[] collide = PhysicsExtensions.OverlapVisionCone(transform.position, VISION_CONE_ANGLE, VISION_CONE_RANGE, transform.forward, LayerMask.GetMask("Entity"));

        Handles.color = new Color(1, 0, 0, 0.25f);
        //if (collide.Length != 0)
        //{
        //    Handles.color = new Color(0, 1, 0, 0.25f);
        //}

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, VISION_CONE_ANGLE / 2f, VISION_CONE_RANGE);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -VISION_CONE_ANGLE / 2f, VISION_CONE_RANGE);
    }
#endif
}