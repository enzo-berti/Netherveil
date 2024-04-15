using UnityEngine;
using UnityEngine.UI;

public abstract class Npc : Entity, IInterractable
{
    public Image rangeImage;
    PlayerInteractions playerInteractions;
    Hero hero;

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
            rangeImage.gameObject.SetActive(false);
        }
    }
}
