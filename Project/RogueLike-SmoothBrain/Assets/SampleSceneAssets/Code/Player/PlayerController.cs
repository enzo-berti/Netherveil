using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Transform cameraTransform;
    [Range(0f, 20f), SerializeField]
    float dashSpeed;

    [SerializeField]
    BoxCollider spearAttack1;
    
    Vector2 direction = Vector2.zero;
    CharacterController characterController;
    readonly float smoothTime = 0.05f;
    float currentVelocity = 0f;
    float currentTargetAngle = 0f;
    public Vector2 dashDir = Vector2.zero;
    public Vector2 LastDir { get; set; } = Vector2.zero;
    public int ComboCount { get; set; } = 0;
    public readonly int MAX_COMBO_COUNT = 3;

    public Hero hero;

    public Vector2 Direction
    {
        get { return direction; }
    }

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
        currentTargetAngle = transform.eulerAngles.y;
    }

    void Update()
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, currentTargetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Move();
        DashMove();

        //Collider[] tab = CheckAttackCollide(spearAttack1, LayerMask.GetMask("Entity"));
        //if(tab != null) 
        //{
        //    foreach(Collider col in tab) 
        //    {
        //        Debug.Log(col.gameObject.name);
        //    }
        //}
    }

    void Move()
    {
        if (hero.State == (int)Entity.EntityState.MOVE && (direction.x != 0f || direction.y != 0f))
        {
            LastDir = direction;
            currentTargetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cameraTransform.rotation.eulerAngles.y;
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            characterController.Move(hero.Stats.GetValueStat(Stat.SPEED) * Time.deltaTime * (camForward * direction.y + camRight * direction.x).normalized);
        }
    }

    void DashMove()
    {
        if (hero.State == (int)Hero.PlayerState.DASH)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            characterController.Move(dashSpeed * Time.deltaTime * (camForward * dashDir.y + camRight * dashDir.x).normalized);
        }
    }

    public void ReadDirection(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
    }

    public Collider[] CheckAttackCollide(Collider collider, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (collider != null)
        {
            System.Type colliderType = collider.GetType();

            switch (colliderType.Name)
            {
                case nameof(BoxCollider):
                    return (collider as BoxCollider).BoxOverlap(layerMask, queryTriggerInteraction);
                case nameof(SphereCollider):
                    return (collider as SphereCollider).SphereOverlap(layerMask, queryTriggerInteraction);
                case nameof(CapsuleCollider):
                    return (collider as CapsuleCollider).CapsuleOverlap(layerMask, queryTriggerInteraction);
                default:
                    Debug.LogWarning("Invalid Collider type, can't check the collision.");
                    return new Collider[0];
            }
        }

        Debug.LogWarning("Collider is null.");
        return new Collider[0];
    }
}