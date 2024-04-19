using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using System;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;
    protected Renderer mRenderer;
    private Material hitMaterial;
    protected Entity[] nearbyEntities;
    protected EnemyLifeBar lifeBar;
    [SerializeField] protected Drop drop;
    public VisualEffect StatSuckerVFX;

    // opti
    protected int frameToUpdate;
    protected int maxFrameUpdate = 500;

    // getters/setters
    public NavMeshAgent Agent { get => agent; }
    public float DamageTakenMultiplicator { get; set; } = 1f;

    protected void OnEnable()
    {

    }

    protected void OnDisable()
    {

    }

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        mRenderer = GetComponentInChildren<Renderer>();
        lifeBar = GetComponentInChildren<EnemyLifeBar>();
        lifeBar.SetMaxValue(stats.GetValue(Stat.HP));

        foreach (Material mat in mRenderer.materials)
        {
            if (mat.HasInt("_isHit"))
            {
                hitMaterial = mat;
            }
        }

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
        this.transform.parent.position = Vector3.zero;
        this.transform.parent.localPosition = Vector3.zero;
        this.transform.localPosition = pos;


        StatSuckerVFX.SetVector3("Attract Target", GameObject.FindWithTag("Player").transform.position + Vector3.up);
        StatSuckerVFX.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXPositionBinderCustom>().ToArray()[0].Target = GameObject.FindWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        if (transform.position.y < -100f)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void ApplySpeed(Stat speedStat)
    {
        if (speedStat != Stat.SPEED || agent == null)
            return;

        agent.speed = stats.GetValue(Stat.SPEED);
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
        hitMaterial.SetInt("_isHit", 1);
        yield return new WaitForSeconds(0.05f);
        hitMaterial.SetInt("_isHit", 0);
        yield return new WaitForSeconds(0.05f);
        hitMaterial.SetInt("_isHit", 1);
        yield return new WaitForSeconds(0.05f);
        hitMaterial.SetInt("_isHit", 0);
    }

    // Je le laisse là ça servira peut être

    //protected void ApplyDamagesMob(int _value, EventReference hitSound, Action deathMethod, bool notEffectDamage)
    //{
    //    // Some times, this method is called when entity is dead ??
    //    if (stats.GetValue(Stat.HP) <= 0 || IsInvincibleCount > 0)
    //        return;

    //    Stats.DecreaseValue(Stat.HP, _value, false);
    //    lifeBar.ValueChanged(stats.GetValue(Stat.HP));

    //    if (notEffectDamage)
    //    {
    //        //add SFX here
    //        FloatingTextGenerator.CreateDamageText(_value, transform.position);
    //        StartCoroutine(HitRoutine());
    //    }

    //    if (stats.GetValue(Stat.HP) <= 0)
    //    {
    //        deathMethod();
    //    }
    //    else
    //    {
    //        AudioManager.Instance.PlaySound(hitSound, transform.position);
    //    }
    //}

    protected void ApplyDamagesMob(int _value, Sound hitSound, Action deathMethod, bool notEffectDamage, bool _restartSound = true)
    {
        // Some times, this method is called when entity is dead ??
        if (stats.GetValue(Stat.HP) <= 0 || IsInvincibleCount > 0)
            return;

        Stats.DecreaseValue(Stat.HP, _value * DamageTakenMultiplicator, false);
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
    
    protected virtual void DisplayAttackRange(float _angle, float _range)
    {
        Handles.color = new Color(1, 1, 0.5f, 0.2f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)_range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)_range);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)_range);
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