using UnityEngine;

public class Spear : MonoBehaviour
{
    bool isThrew = false;
    bool isThrowing = false;

    Transform player;
    Transform parent = null;

    [SerializeField] GameObject trailPf;
    GameObject trail;

    Quaternion initLocalRotation;
    Vector3 initLocalPosition;

    Vector3 spearPosition;
    public bool IsThrew
    {
        get
        {
            return (isThrew);
        }
    }
    public bool IsThrowing
    {
        get
        {
            return (isThrowing);
        }
    }
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
        if (isThrew && (this.player.position - posToReach).magnitude < (this.player.position - trail.transform.position).magnitude)
        {
            Destroy(trail);
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            // We set position at the exact place ( the spear doesn't move, just tp )
            this.gameObject.transform.position = posToReach;
            this.gameObject.transform.rotation = Quaternion.identity * Quaternion.Euler(90,0,0);
            isThrowing = false;
        }
        else if(!isThrew && parent != null && (spearPosition - posToReach).magnitude < (spearPosition - trail.transform.position).magnitude)
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
            isThrowing = false;
            Destroy(trail);
        }
    }

    public void Throw(Vector3 _posToReach)
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - this.transform.position).normalized * 5000, ForceMode.Force);
        RaycastHit[] hits = Physics.BoxCastAll((posToReach - this.transform.position) / 2, new Vector3(1, 1, (posToReach - this.transform.position).magnitude / 2), (posToReach - this.transform.position));
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if(hit.collider.gameObject.TryGetComponent<IDamageable>(out _))
                Debug.Log($"damage on {hit.collider.name}");
            }
        }

        // On set le parent que la lance avait ( la main du joueur ), puis on la retire tant qu'elle est lancée afin de la rendre indépendante 
        parent = this.transform.parent;
        this.transform.parent = null;

        isThrew = true;
        isThrowing = true;

    }

    public void Return()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, posToReach, Quaternion.identity);
        // Spear position est la position où la lance était plantée avant de revenir vers le joueur
        spearPosition = posToReach;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * 5000, ForceMode.Force);

        isThrew = false;
        isThrowing = true;
    }
}
