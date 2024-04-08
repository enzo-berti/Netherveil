using System;
using System.Collections;
using UnityEngine;

public class Spear : MonoBehaviour
{
    Transform player;
    Hero hero;
    Transform parent = null;
    Animator playerAnimator;
    public static event Action OnPlacedInWorld;
    public static event Action OnPlacedInHand;

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
    readonly float SPEAR_SPEED = 5000f;
    readonly float SPEAR_WAIT_TIME = 0.15f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (spearThrowCollider == null)
        {
            spearThrowCollider = player.GetComponent<PlayerController>().GetSpearThrowCollider();
        }
        hero = player.GetComponent<Hero>();
        initLocalRotation = transform.localRotation;
        initLocalPosition = transform.localPosition;
        playerAnimator = player.GetComponentInChildren<Animator>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnDestroy()
    {
        if(trail) Destroy(trail);
        StopAllCoroutines();
    }

    void Update()
    {
        playerAnimator.SetBool("SpearThrowing", IsThrowing);
        playerAnimator.SetBool("SpearThrown", IsThrown);

        if (trail == null)
        {
            return;
        }


        if (CanPlaceSpearInWorld())
        {
            PlaceSpearInWorld();
        }
        else if (CanPlaceSpearInHand())
        {
            PlaceSpearInPlayerHand();
        }
    }

    private void PlaceSpearInPlayerHand()
    {
        OnPlacedInHand?.Invoke();
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
        if (hero.State != (int)Hero.PlayerState.KNOCKBACK)
        {
            hero.State = (int)Entity.EntityState.MOVE;
        }
        spearThrowCollider.gameObject.SetActive(false);
    }

    private void PlaceSpearInWorld()
    {
        Debug.Log("zoubi");
        Destroy(trail);
        meshRenderer.enabled = true;
        // We set position at the exact place ( the spear doesn't move, just tp )
        this.transform.rotation = Quaternion.identity * Quaternion.Euler(-90f, 90f, 0);
        this.transform.position = new Vector3(posToReach.x, player.position.y + GetComponent<BoxCollider>().bounds.size.z / 2f, posToReach.z);
        IsThrowing = false;
        if (hero.State != (int)Hero.PlayerState.KNOCKBACK)
        {
            hero.State = (int)Entity.EntityState.MOVE;
        }
        spearThrowCollider.gameObject.SetActive(false);

        OnPlacedInWorld?.Invoke();
    }

    public void Throw(Vector3 _posToReach)
    {
        StartCoroutine(ThrowCoroutine(_posToReach));
    }


    IEnumerator ThrowCoroutine(Vector3 _posToReach)
    {
        IsThrown = true;
        IsThrowing = true;
        hero.State = (int)Entity.EntityState.ATTACK;

        yield return new WaitForSeconds(SPEAR_WAIT_TIME);

        AudioManager.Instance.PlaySound(player.GetComponent<PlayerController>().ThrowSpearSFX);
        meshRenderer.enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        Vector3 playerToPosToReachVec = (posToReach - this.transform.position);

        trail.GetComponent<Rigidbody>().AddForce(playerToPosToReachVec.normalized * SPEAR_SPEED, ForceMode.Force);
        DeviceManager.Instance.ApplyVibrations(0.001f, 0.005f, 0.1f);

        //check if colliding with obstacle to stop the spear on collide
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, (posToReach - this.transform.position), (posToReach - this.transform.position).magnitude);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & LayerMask.GetMask("Map")) != 0)
                {
                    posToReach = new Vector3(hit.point.x, player.position.y, hit.point.z);
                    playerToPosToReachVec = (posToReach - this.transform.position);
                    break;
                }
            }
        }

        ApplyDamages(playerToPosToReachVec, debugMode: false);

        // On set le parent que la lance avait ( la main du joueur ), puis on la retire tant qu'elle est lancée afin de la rendre indépendante 
        parent = this.transform.parent;
        this.transform.parent = null;
    }

    public void Return()
    {
        StartCoroutine(ReturnCoroutine());
    }

    IEnumerator ReturnCoroutine()
    {
        IsThrown = false;
        IsThrowing = true;
        hero.State = (int)Entity.EntityState.ATTACK;

        yield return new WaitForSeconds(SPEAR_WAIT_TIME);

        AudioManager.Instance.PlaySound(player.GetComponent<PlayerController>().RetrieveSpearSFX);
        meshRenderer.enabled = false;
        trail = Instantiate(trailPf, posToReach, Quaternion.identity);
        // Spear position est la position où la lance était plantée avant de revenir vers le joueur
        spearPosition = posToReach;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * SPEAR_SPEED, ForceMode.Force);
        DeviceManager.Instance.ApplyVibrations(0.005f, 0.001f, 0.1f);

        //orient player in front of spear
        float angle = player.AngleOffsetToFaceTarget(new Vector3(spearPosition.x, player.position.y, spearPosition.z));
        if (angle != float.MaxValue)
        {
            player.GetComponent<PlayerController>().OffsetPlayerRotation(angle, true);
        }

        Vector3 playerToSpearVec = spearPosition - player.position;
        ApplyDamages(playerToSpearVec, debugMode: false);
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
                    hero.Attack(entity);
                }
            }
        }
    }

    private bool CanPlaceSpearInHand()
    {
        return !IsThrown && parent != null && (spearPosition - posToReach).magnitude < (spearPosition - trail.transform.position).magnitude;
    }

    private bool CanPlaceSpearInWorld()
    {
        return IsThrown && (this.player.position - posToReach).magnitude < (this.player.position - trail.transform.position).magnitude;
    }
}
