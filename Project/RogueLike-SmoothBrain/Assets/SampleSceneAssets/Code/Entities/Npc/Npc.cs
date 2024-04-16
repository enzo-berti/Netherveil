using UnityEngine;
using UnityEngine.UI;

public abstract class Npc : Entity, IInterractable
{
    [SerializeField] Image rangeImage;
    public Image RangeImage { get => rangeImage; }
    PlayerInteractions playerInteractions;
    Hero hero;
    private bool isSelect = false;

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
            
        }
    }

    public void ToggleRangeImage(bool toggle)
    {
        float durationScale = 1.0f;
        StopAllCoroutines();
        StartCoroutine(toggle ? EasingFunctions.UpScaleCoroutine(rangeImage.gameObject, durationScale) : EasingFunctions.DownScaleCoroutine(rangeImage.gameObject, durationScale));
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
