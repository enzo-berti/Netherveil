using PostProcessingEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : ConstantStatus
{
    float baseAgentSpeed;
    Material freezeMat = null;
    public Freeze(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = false;
    }

    public override Status DeepCopy()
    {
        Freeze freeze = (Freeze)MemberwiseClone();
        return freeze;
    }

    protected override void Effect()
    {
        if (target != null)
        {
            Debug.Log(target.Stats.GetValue(Stat.SPEED));
            baseAgentSpeed = target.Stats.GetValue(Stat.SPEED);
            target.Stats.SetValue(Stat.SPEED, 0);
            target.isFreeze = true;
        }
    }

    public override void OnFinished()
    {
        target.Stats.SetValue(Stat.SPEED, baseAgentSpeed);
        target.isFreeze = false;
        Renderer renderer = target.GetComponentInChildren<Renderer>();
        List<Material> materials = new List<Material>(renderer.materials);
        materials.RemoveAll(mat => mat.shader == freezeMat.shader);
        renderer.SetMaterials(materials);

        if (Utilities.IsPlayer(target))
            PostProcessingEffectManager.current.Stop(PostProcessingEffects.Effect.Freeze);
    }

    public override bool CanApplyEffect(Entity target)
    {
        return target.Stats.HasStat(Stat.SPEED);
    }

    protected override void PlayStatus()
    {
        PlayVFX();
        PlayPostProcessing();
    }
    
    private void PlayVFX()
    {
        PlayVfx("VFX_Frozen");

        freezeMat = GameResources.Get<Material>("OutlineShaderMat");
        Renderer renderer = target.GetComponentInChildren<Renderer>();

        List<Material> materials = new List<Material>(renderer.materials)
            {
                freezeMat
            };
        renderer.SetMaterials(materials);
    }

    private void PlayPostProcessing()
    {
        if (Utilities.IsPlayer(target))
            PostProcessingEffectManager.current.Play(PostProcessingEffects.Effect.Freeze);
    }
}
