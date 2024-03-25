using UnityEngine;

public class Spear : MonoBehaviour
{
    Transform player;
    Hero hero;
    Transform parent = null;
    Animator playerAnimator;

    [SerializeField] GameObject trailPf;
    [SerializeField] BoxCollider spearThrowCollider;
    GameObject trail;

    Quaternion initLocalRotation;
    Vector3 initLocalPosition;

    Vector3 spearPosition;
    public bool IsThrown { get; private set; } = false;
    public bool IsThrowing { get; private set; } = false;
    Vector3 posToReach = new();
    MeshRenderer meshRenderer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hero = player.GetComponent<Hero>();
        initLocalRotation = transform.localRotation;
        initLocalPosition = transform.localPosition;
        playerAnimator = player.GetComponentInChildren<Animator>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        playerAnimator.SetBool("SpearThrowing", IsThrowing);
        playerAnimator.SetBool("SpearThrown", IsThrown);

        if (trail == null)
        {
            return;
        }


        if (IsThrown && (this.player.position - posToReach).magnitude < (this.player.position - trail.transform.position).magnitude)
        {
            Destroy(trail);
            meshRenderer.enabled = true;
            // We set position at the exact place ( the spear doesn't move, just tp )
            this.transform.position = posToReach;
            this.transform.rotation = Quaternion.identity * Quaternion.Euler(-90, 0, 0);
            IsThrowing = false;
            if(hero.State != (int)Hero.PlayerState.KNOCKBACK)
            {
                hero.State = (int)Entity.EntityState.MOVE;
            }
            spearThrowCollider.gameObject.SetActive(false);
        }
        else if (!IsThrown && parent != null && (spearPosition - posToReach).magnitude < (spearPosition - trail.transform.position).magnitude)
        {
            this.transform.position = posToReach;
            // On r�atache la lance � la main
            this.transform.SetParent(parent, true);
            // On r�init la local pos et la local rotation pour que la lance soit parfaitement dans la main du joueur comme elle l'�tait
            this.transform.localPosition = initLocalPosition;
            this.transform.localRotation = initLocalRotation;
            parent = null;
            meshRenderer.enabled = true;
            IsThrowing = false;
            Destroy(trail);
            if (hero.State != (int)Hero.PlayerState.KNOCKBACK)
            {
                hero.State = (int)Entity.EntityState.MOVE;
            }
            spearThrowCollider.gameObject.SetActive(false);
        }
    }

    public void Throw(Vector3 _posToReach)
    {
        meshRenderer.enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        Vector3 playerToPosToReachVec = (posToReach - this.transform.position);

        trail.GetComponent<Rigidbody>().AddForce(playerToPosToReachVec.normalized * 5000, ForceMode.Force);
        DeviceManager.Instance.ApplyVibrations(0.001f, 0.005f, 0.1f);

        //check if colliding with obstacle to stop the spear on collide
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, (posToReach - this.transform.position), (posToReach - this.transform.position).magnitude);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & LayerMask.GetMask("Map")) != 0)
                {
                    posToReach = hit.point;
                    playerToPosToReachVec = (posToReach - this.transform.position);
                    break;
                }
            }
        }

        ApplyDamages(playerToPosToReachVec, debugMode: true);

        // On set le parent que la lance avait ( la main du joueur ), puis on la retire tant qu'elle est lanc�e afin de la rendre ind�pendante 
        parent = this.transform.parent;
        this.transform.parent = null;

        IsThrown = true;
        IsThrowing = true;
        hero.State = (int)Entity.EntityState.ATTACK;
    }

    public void Return()
    {
        meshRenderer.enabled = false;
        trail = Instantiate(trailPf, posToReach, Quaternion.identity);
        // Spear position est la position o� la lance �tait plant�e avant de revenir vers le joueur
        spearPosition = posToReach;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * 5000, ForceMode.Force);
        DeviceManager.Instance.ApplyVibrations(0.005f, 0.001f, 0.1f);

        //orient player in front of spear
        float angle = player.AngleOffsetToFaceTarget(new Vector3(spearPosition.x, player.position.y, spearPosition.z));
        if (angle != float.MaxValue)
        {
            player.GetComponent<PlayerController>().OffsetPlayerRotation(angle, true);
        }

        Vector3 playerToSpearVec = spearPosition - player.position;
        ApplyDamages(playerToSpearVec, debugMode: true);

        IsThrown = false;
        IsThrowing = true;
        hero.State = (int)Entity.EntityState.ATTACK;
    }

    void ApplyDamages(Vector3 playerToTargetPos, bool debugMode)
    {
        if(debugMode)
        {
            spearThrowCollider.gameObject.SetActive(true);
        }

        //offset so that the collide also takes the spear end spot
        float collideOffset = 0.2f;
        //construct collider in scene so that we can debug it
        Vector3 scale = spearThrowCollider.transform.localScale;
        scale.z = playerToTargetPos.magnitude;
        spearThrowCollider.transform.localScale = scale;
        spearThrowCollider.transform.localPosition = new Vector3(0f, 0f, scale.z / 2f + collideOffset);

        Collider[] colliders = spearThrowCollider.BoxOverlap();

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != player.gameObject)
                {
                    entity.ApplyDamage((int)(hero.Stats.GetValue(Stat.ATK) * hero.Stats.GetValue(Stat.ATK_COEFF)));
                }
            }
        }
    }
}
