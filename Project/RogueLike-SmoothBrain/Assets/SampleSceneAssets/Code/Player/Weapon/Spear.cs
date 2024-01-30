using UnityEngine;

public class Spear : MonoBehaviour
{
    Transform player;
    Transform parent = null;

    [SerializeField] GameObject trailPf;
    GameObject trail;

    Quaternion initLocalRotation;
    Vector3 initLocalPosition;

    Vector3 spearPosition;
    public bool IsThrown { get; private set; } = false;
    public bool IsThrowing { get; private set; } = false;
    Vector3 posToReach = new();
    Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        initLocalRotation = transform.localRotation;
        initLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (trail == null)
        {
            return;
        }
        if (IsThrown && (this.player.position - posToReach).magnitude < (this.player.position - trail.transform.position).magnitude)
        {
            Destroy(trail);
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            // We set position at the exact place ( the spear doesn't move, just tp )
            this.gameObject.transform.position = posToReach;
            this.gameObject.transform.rotation = Quaternion.identity * Quaternion.Euler(90,0,0);
            IsThrowing = false;
        }
        else if(!IsThrown && parent != null && (spearPosition - posToReach).magnitude < (spearPosition - trail.transform.position).magnitude)
        {
            rb.velocity = Vector3.zero;
            this.gameObject.transform.position = posToReach;
            // On réatache la lance à la main
            this.transform.SetParent(parent, true);
            // On réinit la local pos et la local rotation pour que la lance soit parfaitement dans la main du joueur comme elle l'était
            this.gameObject.transform.localPosition = initLocalPosition;
            this.gameObject.transform.localRotation = initLocalRotation;
            parent = null;
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            IsThrowing = false;
            Destroy(trail);
        }
    }

    public void Throw(Vector3 _posToReach)
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - this.transform.position).normalized * 5000, ForceMode.Force);
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, (posToReach - this.transform.position), (posToReach - this.transform.position).magnitude);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if(hit.collider.gameObject.TryGetComponent<IDamageable>(out _))
                    Debug.Log($"damage on {hit.collider.name}");
                //else if (hit.collider.gameObject.layer == LayerMask.GetMask("Map")) 
                //    trail.
            }
        }

        // On set le parent que la lance avait ( la main du joueur ), puis on la retire tant qu'elle est lancée afin de la rendre indépendante 
        parent = this.transform.parent;
        this.transform.parent = null;

        IsThrown = true;
        IsThrowing = true;

    }

    public void Return()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, posToReach, Quaternion.identity);
        // Spear position est la position où la lance était plantée avant de revenir vers le joueur
        spearPosition = posToReach;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * 5000, ForceMode.Force);

        IsThrown = false;
        IsThrowing = true;
    }
}
