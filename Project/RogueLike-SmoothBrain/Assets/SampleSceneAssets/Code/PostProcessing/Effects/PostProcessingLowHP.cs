using PostProcessingEffects;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingLowHP : MonoBehaviour
{
    private bool activePostProcessing;
    [SerializeField] private PostProcessingHitEffect hitEffect;

    [SerializeField] private Volume volume;
    public Coroutine routine = null;
    public bool effectIsPlaying = false;

    public Volume Volume => volume;

    void Start()
    {
        activePostProcessing = false;
        Utilities.Hero.OnTakeDamage += Active;
        Utilities.Hero.OnHeal += Desactive;
    }

    void Update()
    {
        if (activePostProcessing)
        {
            if (!effectIsPlaying)
            {
                routine = StartCoroutine(hitEffect.PlayRoutine(this));
            }
        }
    }

    private void Active(int _damage, IAttacker _attackAutor)
    {
        if (Utilities.Hero.Stats.GetValue(Stat.HP) <= (Utilities.Hero.Stats.GetMaxValue(Stat.HP) / 4f))
        {
            activePostProcessing = true;
        }
    }

    private void Desactive(int _healingAmount)
    {
        if (Utilities.Hero.Stats.GetValue(Stat.HP) > (Utilities.Hero.Stats.GetMaxValue(Stat.HP) / 4f))
        {
            activePostProcessing = false;
        }
    }
}
