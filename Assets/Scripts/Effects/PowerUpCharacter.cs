using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PowerUpCharacter : Character
{
    private Tween _tween;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        LevelCompleteAnimation.OnEndLevelUIOpen += () => Destroy(gameObject);
        SetSearchable();
        
        _tween = DOVirtual.DelayedCall( 10, () =>
        {
            var particles = Instantiate(GameManager.Instance.BadEffectParticles, transform);
            particles.transform.localPosition = new Vector3(0, 0.5f, 0);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
            
            EffectsManager.Instance.GiveEffectToPlayer(transform, EffectsManager.TypeOfCharEffect.BadEffect);

            SetUnsearchable();
        });
    }
    
    protected override void SetSearchableUI()
    {
        
    }

    protected override void SetUnsearchableUI()
    {
        EffectsManager.Instance.GiveEffectToPlayer(transform, EffectsManager.TypeOfCharEffect.GoodEffect); 
        _tween?.Kill();
    }
}
