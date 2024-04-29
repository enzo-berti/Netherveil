using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using System.Linq;
using Map;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(HitMaterialApply))]
public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;
    protected Renderer mRenderer;
    protected Entity[] nearbyEntities;
    protected EnemyLifeBar lifeBar;
    protected Animator animator;
    [SerializeField] protected Drop drop;
    public VisualEffect StatSuckerVFX;
    [SerializeField] protected VisualEffect spawningVFX;
    private HitMaterialApply hit;
    private Material spawningMat;

    // opti
    protected int frameToUpdate;
    protected int maxFrameUpdate = 20;
    public bool IsSpawning { get; protected set; } = true;

    // extra
    [Serializable]
    public struct Zone
    {
        [HideInInspector]
        public Vector3 center;
        public int radius;
    }
    [SerializeField] protected Zone wanderZone = new() { radius = 8 };

    // getters/setters
    public NavMeshAgent Agent { get => agent; }
    public float DamageTakenMultiplicator { get; set; } = 1f;
    public Vector3 WanderZoneCenter { get => wanderZone.center; set => wanderZone.center = value; }
    public int WanderZoneRadius { get => wanderZone.radius; set => wanderZone.radius = value; }

    protected virtual void OnEnable()
    {
        RoomUtilities.onEarlyEnter += OnEarlyEnterRoom;
    }

    protected virtual void OnDisable()
    {
        RoomUtilities.onEarlyEnter -= OnEarlyEnterRoom;
    }

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        mRenderer = GetComponentInChildren<Renderer>();
        lifeBar = GetComponentInChildren<EnemyLifeBar>();
        animator = GetComponentInChildren<Animator>();
        hit = GetComponentInChildren<HitMaterialApply>();
        lifeBar.SetMaxValue(stats.GetValue(Stat.HP));

        nearbyEntities = null;
        ApplySpeed(Stat.SPEED);
        stats.onStatChange += ApplySpeed;
        OnDeath += cts => ClearStatus();
        OnDeath += drop.DropLoot;

        if (this is IAttacker attacker)
        {
            attacker.OnAttackHit += attacker.ApplyStatus;
        }

        StartCoroutine(EntityDetection());
        StartCoroutine(Brain());

        Vector3 pos = this.transform.parent.localPosition;
        Quaternion rot = this.transform.parent.localRotation;
        this.transform.parent.localRotation = Quaternion.identity;
        this.transform.parent.position = Vector3.zero;
        this.transform.parent.localPosition = Vector3.zero;
        this.transform.localPosition = pos;
        this.transform.localRotation = rot;

        StatSuckerVFX.SetVector3("Attract Target", GameObject.FindWithTag("Player").transform.position + Vector3.up);
        StatSuckerVFX.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXPositionBinderCustom>().ToArray()[0].Target = GameObject.FindWithTag("Player").transform;

        animator.speed = 0;
        IsInvincibleCount++;
        IsKnockbackable = false;
        lifeBar.gameObject.SetActive(false);
        spawningVFX.GetComponent<VFXStopper>().Duration = spawningVFX.GetFloat("Duration") + 0.5f;
        spawningVFX.GetComponent<VFXStopper>().PlayVFX();
        spawningVFX.GetComponent<VFXStopper>().OnStop.AddListener(EndOfSpawningVFX);

        AddSpawningMat();
        wanderZone.center = transform.position;
    }

    private void AddSpawningMat()
    {
        spawningMat = GameResources.Get<Material>("MAT_VFX_Spawn");

        if (GetComponentsInChildren<SkinnedMeshRenderer>().Length == 0)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                List<Material> materials = new List<Material>(renderer.materials)
            {
                spawningMat
            };
                renderer.SetMaterials(materials);
            }
        }
        else
        {
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                List<Material> materials = new List<Material>(renderer.materials)
            {
                spawningMat
            };
                renderer.SetMaterials(materials);
            }
        }

        StartCoroutine(SpawningMatCoroutine());
    }

    protected override void Update()
    {
        base.Update();
        if (transform.position.y < -100f)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnEarlyEnterRoom()
    {
        if (Utilities.Hero.Stats.GetValue(Stat.CORRUPTION) <= -100f)
        {
            GameObject clone = Instantiate(transform.parent.gameObject, transform.parent.parent);
            RoomUtilities.roomData.enemies.Add(clone);
        }
    }

    private void ApplySpeed(Stat speedStat)
    {
        if (speedStat != Stat.SPEED || agent == null)
            return;

        agent.speed = stats.GetValue(Stat.SPEED);
    }

    private void EndOfSpawningVFX()
    {
        IsInvincibleCount--;
        spawningVFX.Stop();
        lifeBar.gameObject.SetActive(true);
        animator.speed = 1;
        IsSpawning = false;
        IsKnockbackable = true;
    }

    private IEnumerator SpawningMatCoroutine()
    {
        if (GetComponentsInChildren<SkinnedMeshRenderer>().Length == 0)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                Material spawningMaterial = renderer.materials.FirstOrDefault(x => x.shader == spawningMat.shader);
                while (spawningMaterial.GetFloat("_Alpha") > 0f)
                {
                    spawningMaterial.SetFloat("_Alpha", spawningMaterial.GetFloat("_Alpha") - Time.deltaTime / spawningVFX.GetComponent<VFXStopper>().Duration);
                    spawningMaterial.SetFloat("_Alpha", Mathf.Max(spawningMaterial.GetFloat("_Alpha"), 0f));;
                    yield return null;
                }

                List<Material> materials = new(renderer.materials);
                materials.RemoveAll(mat => mat.shader == spawningMat.shader);
                renderer.SetMaterials(materials);
            }
        }
        else
        {
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                Material spawningMaterial = renderer.materials.FirstOrDefault(x => x.shader == spawningMat.shader);
                while (spawningMaterial.GetFloat("_Alpha") > 0f)
                {
                    spawningMaterial.SetFloat("_Alpha", spawningMaterial.GetFloat("_Alpha") - Time.deltaTime / spawningVFX.GetComponent<VFXStopper>().Duration);
                    spawningMaterial.SetFloat("_Alpha", Mathf.Max(spawningMaterial.GetFloat("_Alpha"), 0f));
                    yield return null;
                }

                List<Material> materials = new(renderer.materials);
                materials.RemoveAll(mat => mat.shader == spawningMat.shader);
                renderer.SetMaterials(materials);
            }
        }
    }

    protected virtual IEnumerator Brain()
    {
        yield return null;
    }

    protected virtual IEnumerator EntityDetection()
    {
        yield return null;
    }

    protected IEnumerator HitRoutine()
    {
        hit.EnableMat();

        hit.SetAlpha(1.0f);
        yield return new WaitForSeconds(0.05f);
        hit.SetAlpha(0.0f);
        yield return new WaitForSeconds(0.05f);
        hit.SetAlpha(1.0f);
        yield return new WaitForSeconds(0.05f);
        hit.SetAlpha(0.0f);

        hit.DisableMat();
    }

    protected void ApplyDamagesMob(int _value, Sound hitSound, Action deathMethod, bool notEffectDamage, bool _restartSound = true)
    {
        // Some times, this method is called when entity is dead ??
        if (stats.GetValue(Stat.HP) <= 0 || IsInvincibleCount > 0)
            return;

        _value = (int)(_value * DamageTakenMultiplicator);
        Stats.DecreaseValue(Stat.HP, _value, false);
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));

        if (notEffectDamage)
        {
            //add SFX here
            FloatingTextGenerator.CreateDamageText(_value, transform.position);
            StartCoroutine(HitRoutine());
        }

        if (stats.GetValue(Stat.HP) <= 0)
        {
            deathMethod();
        }
        else
        {
            hitSound.Play(transform.position, _restartSound);
        }
    }

    public Vector3 GetRandomPointOnWanderZone(Vector3 _unitPos, float _minTravelDistance, float _maxTravelDistance, bool _avoidWalls = true)
    {
        if (_minTravelDistance >= _maxTravelDistance)
        {
            Debug.LogError("Invalid min/max value. Returning (0,0,0).");
            return default;
        }

        Vector3 randomDirection3D = default;

        for (int i = 0; i < 3; i++)
        {
            bool validPoint = true;

            Vector2 randomDirection2D = UnityEngine.Random.insideUnitCircle;
            randomDirection2D *= UnityEngine.Random.Range(_minTravelDistance, _maxTravelDistance);
            randomDirection3D = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);

            if (_avoidWalls)
            {
                if (Physics.Raycast(_unitPos + new Vector3(0, 1, 0), randomDirection3D.normalized, randomDirection3D.magnitude, LayerMask.GetMask("Map")))
                {
                    validPoint = false;
                }
            }

            if ((_unitPos + randomDirection3D - wanderZone.center).sqrMagnitude < wanderZone.radius * wanderZone.radius && validPoint)
            {
                return _unitPos + randomDirection3D;
            }
        }

        return (_unitPos - wanderZone.center).normalized * _minTravelDistance;
    }

#if UNITY_EDITOR
    protected virtual void DisplayVisionRange(float _angle)
    {
        Handles.color = new Color(1, 0, 0, 0.2f);
        if (nearbyEntities != null && nearbyEntities.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.2f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)stats.GetValue(Stat.VISION_RANGE));
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)stats.GetValue(Stat.VISION_RANGE));

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)stats.GetValue(Stat.VISION_RANGE));
    }

    protected virtual void DisplayVisionRange(float _angle, float _range)
    {
        Handles.color = new Color(1, 0, 0, 0.2f);
        if (nearbyEntities != null && nearbyEntities.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.2f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)_range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)_range);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)_range);
    }

    protected virtual void DisplayAttackRange(float _angle)
    {
        Handles.color = new Color(1, 1, 0.5f, 0.2f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)stats.GetValue(Stat.ATK_RANGE));
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)stats.GetValue(Stat.ATK_RANGE));

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)stats.GetValue(Stat.ATK_RANGE));
    }

    protected virtual void DisplayAttackRange(float _angle, float _range, bool _int = true)
    {
        float range = _int ? (int)_range : _range;

        Handles.color = new Color(1, 1, 0.5f, 0.2f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, range);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }

    protected virtual void DisplayWanderZone()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(wanderZone.center, Vector3.up, wanderZone.radius, 5);
        Handles.color = new Color(1, 1, 0, 0.1f);
        Handles.DrawSolidDisc(wanderZone.center, Vector3.up, wanderZone.radius);
    }

    protected virtual void DisplayInfos()
    {
        Handles.Label(
        transform.position + transform.up,
        stats.GetEntityName() +
        "\n - Health : " + stats.GetValue(Stat.HP) +
        "\n - Speed : " + stats.GetValue(Stat.SPEED),
        new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState()
            {
                textColor = Color.black
            }
        });
    }
#endif
}