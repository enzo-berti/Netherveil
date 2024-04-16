using UnityEngine;
using UnityEngine.UI;

public abstract class Npc : Entity, IInterractable
{
    [SerializeField] Image rangeImage;
    public Image RangeImage { get => rangeImage; }
    PlayerInteractions playerInteractions;
    Hero hero;
    float factor = 0f;

    public virtual void Interract()
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        base.Start();
        playerInteractions = GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>();
        hero = playerInteractions.GetComponent<Hero>();
        factor = Time.deltaTime * 2f;
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
        StopAllCoroutines();
        StartCoroutine(toggle ? EasingFunctions.UpScaleCoroutine(rangeImage.gameObject, factor) : EasingFunctions.DownScaleCoroutine(rangeImage.gameObject, factor));
    }

    public void Select()
    {
        ToggleRangeImage(true);
    }

    public void Deselect()
    {
        ToggleRangeImage(false);
    }
}
