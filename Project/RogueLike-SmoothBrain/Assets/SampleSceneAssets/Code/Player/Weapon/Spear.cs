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
        playerAnimator = player.GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
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
            this.transform.rotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
            IsThrowing = false;
            hero.State = (int)Entity.EntityState.MOVE;
        }
        else if (!IsThrown && parent != null && (spearPosition - posToReach).magnitude < (spearPosition - trail.transform.position).magnitude)
        {
            this.transform.position = posToReach;
            // On réatache la lance à la main
            this.transform.SetParent(parent, true);
            // On réinit la local pos et la local rotation pour que la lance soit parfaitement dans la main du joueur comme elle l'était
            this.transform.localPosition = initLocalPosition;
            this.transform.localRotation = initLocalRotation;
            parent = null;
            meshRenderer.enabled = true;
            IsThrowing = false;
            Destroy(trail);
            hero.State = (int)Entity.EntityState.MOVE;
        }
    }

    public void Throw(Vector3 _posToReach)
    {
        meshRenderer.enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        Vector3 playerToPosToReachVec = (posToReach - this.transform.position);

        trail.GetComponent<Rigidbody>().AddForce(playerToPosToReachVec.normalized * 5000, ForceMode.Force);
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

        //offset so that the collide also takes the spear end spot
        float collideOffset = 0.3f;
        //construct collider in scene so that we can debug it
        Vector3 scale = spearThrowCollider.transform.localScale;
        scale.z = playerToPosToReachVec.magnitude;
        spearThrowCollider.transform.localScale = scale;
        spearThrowCollider.transform.localPosition = new Vector3(0f, 0f, scale.z / 2f + collideOffset);

        Collider[] colliders = spearThrowCollider.BoxOverlap();

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != player.gameObject)
                {
                    entity.ApplyDamage((int)(hero.Stats.GetValueStat(Stat.ATK) * hero.Stats.GetValueStat(Stat.ATK_COEFF)));
                }
            }
        }

        // On set le parent que la lance avait ( la main du joueur ), puis on la retire tant qu'elle est lancée afin de la rendre indépendante 
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
        // Spear position est la position où la lance était plantée avant de revenir vers le joueur
        spearPosition = posToReach;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * 5000, ForceMode.Force);

        //orient player in front of spear
        float angle = player.AngleOffsetToFaceTarget(new Vector3(spearPosition.x, player.position.y, spearPosition.z));
        if (angle != float.MaxValue)
        {
            player.GetComponent<PlayerController>().OffsetPlayerRotation(angle, true);
        }

        Vector3 playerToSpearVec = spearPosition - player.position;
        //offset so that the collide also takes the spear end spot
        float collideOffset = 0.3f;
        //construct collider in scene so that we can debug it
        Vector3 scale = spearThrowCollider.transform.localScale;
        scale.z = playerToSpearVec.magnitude;
        spearThrowCollider.transform.localScale = scale;
        spearThrowCollider.transform.localPosition = new Vector3(0f, 0f, scale.z / 2f + collideOffset);

        RaycastHit[] hits = spearThrowCollider.BoxCastAll();

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<IDamageable>(out var entity) && hit.collider.gameObject != player.gameObject)
                {
                    entity.ApplyDamage((int)hero.Stats.GetValueStat(Stat.ATK));
                }
            }
        }

        IsThrown = false;
        IsThrowing = true;
        hero.State = (int)Entity.EntityState.ATTACK;
    }
}
