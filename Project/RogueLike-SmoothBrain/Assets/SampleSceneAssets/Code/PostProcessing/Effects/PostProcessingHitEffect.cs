using PostProcessingEffects.Effects;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostProcessingEffects
{
    [System.Serializable]
    public class PostProcessingHitEffect : PostProcessingAbstractEffect
    {
        [SerializeField] private VolumeProfile profile;
        [SerializeField] private float duration = 0.5f;

        protected override IEnumerator PlayRoutine(PostProcessingEffectManager manager)
        {
            manager.Volume.profile = profile;
            manager.effectIsPlaying = true;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
                float factor = elapsed / duration;
                float ease = Mathf.Sin(factor * Mathf.PI);

                manager.Volume.weight = ease;

                yield return null;
            }

            manager.Volume.weight = 0.0f;
            manager.routine = null;
            manager.effectIsPlaying = false;
        }

        protected override IEnumerator StopRoutine(PostProcessingEffectManager manager)
        {
            throw new System.NotImplementedException();
        }
    }
}
