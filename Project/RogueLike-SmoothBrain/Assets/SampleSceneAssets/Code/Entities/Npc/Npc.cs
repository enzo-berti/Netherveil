using UnityEngine;
using UnityEngine.UI;

public abstract class Npc : Entity, IInterractable
{
    [SerializeField] public Image rangeImage;
    public Image RangeImage { get => rangeImage; }
    PlayerInteractions playerInteractions;
    Hero hero;
    private bool isSelect = false;
    private Coroutine rangeRoutine;

    public virtual void Interract()
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        base.Start();
        playerInteractions = GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>();
        hero = playerInteractions.GetComponent<Hero>();
    }

    protected override void Update()
    {
        base.Update();
        Interraction();
    }

    private void OnDisable()
    {
        if (rangeRoutine != null)
        {
            StopCoroutine(rangeRoutine);
            rangeImage.rectTransform.localScale = Vector3.zero;
        }
    }

    private void Interraction()
    {
        bool isInRange = Vector2.Distance(playerInteractions.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2())
            <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

        if (isInRange && !playerInteractions.InteractablesInRange.Contains(this))
        {
            playerInteractions.InteractablesInRange.Add(this);
        }
        else if (!isInRange && playerInteractions.InteractablesInRange.Contains(this))
        {
            playerInteractions.InteractablesInRange.Remove(this);
            Deselect();       
        }
    }

    public void ToggleRangeImage(bool toggle)
    {
        float durationScale = 0.15f;

        if (rangeRoutine != null)
            StopCoroutine(rangeRoutine);

        rangeRoutine = StartCoroutine(toggle ? rangeImage.rectTransform.UpScaleCoroutine(durationScale) : rangeImage.rectTransform.DownScaleCoroutine(durationScale));
    }

    public void Select()
    {
        if (isSelect)
            return;

        isSelect = true;
        ToggleRangeImage(true);
    }

    public void Deselect()
    {
        if (!isSelect)
            return;

        isSelect = false;
        ToggleRangeImage(false);
    }
}
