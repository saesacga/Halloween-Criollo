using DG.Tweening;
using UnityEngine;

public class PowerUpCharacter : Character
{
    protected override void OnEnable()
    {
        base.OnEnable();
        LevelCompleteAnimation.OnEndLevelUIOpen += () =>
        {
            gameObject.SetActive(false);
        };
        PowerUpComingUI.OnPowerUpTimerUp += NegativeEffect;
        
        var spriteRenderer = VisualCharacter.GetComponent<SpriteRenderer>();
        PowerUpComingUI.Instance.SetSprite(spriteRenderer.material, VisualCharacter.GetComponent<Animator>().runtimeAnimatorController, CharacterName);
        
        SetSearchable();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
        PowerUpComingUI.OnPowerUpTimerUp -= NegativeEffect;
    }

    protected override void UnsearchablePolymorph()
    {
        EffectsManager.Instance.GiveEffectToPlayer(transform, EffectsManager.TypeOfCharEffect.GoodEffect); 
        PowerUpComingUI.Instance.HideCharacterPowerUpUI();
        PowerUpComingUI.OnPowerUpTimerUp -= NegativeEffect;
    }

    protected override void SetSearchableUI()
    {
    }

    private void NegativeEffect()
    {
        var particles = Instantiate(GameManager.Instance.BadEffectParticles, transform);
        particles.transform.localPosition = new Vector3(0, 0.5f, 0);
        particles.Play();
        Destroy(particles.gameObject, particles.main.duration);
            
        EffectsManager.Instance.GiveEffectToPlayer(transform, EffectsManager.TypeOfCharEffect.BadEffect);

        SetUnsearchable();
        
        PowerUpComingUI.OnPowerUpTimerUp -= NegativeEffect;
    }
}
