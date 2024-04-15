using System.Collections.Generic;
using UnityEngine;

public abstract class Npc : Entity
{
    PlayerInteractions playerInteractions;
    Hero hero;

    protected override void Start()
    {
        base.Start();
        playerInteractions = GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>();
        hero = playerInteractions.GetComponent<Hero>();
    }

    protected override void Update()
    {
        base.Update();
    }

    //private void Interraction()
    //{
    //    bool isInRange = Vector2.Distance(playerInteractions.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2())
    //        <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

    //    if (isInRange && !playerInteractions.InteractablesInRange.Contains(this))
    //    {
    //        playerInteractions.InteractablesInRange.Add(this);
    //    }
    //    else if (!isInRange && playerInteractions.InteractablesInRange.Contains(this))
    //    {
    //        playerInteractions.InteractablesInRange.Remove(this);
    //        outline.DisableOutline();
    //        itemDescription.TogglePanel(false);
    //    }
    //}
}
