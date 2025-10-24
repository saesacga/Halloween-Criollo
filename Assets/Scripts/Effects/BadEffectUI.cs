using UnityEngine;
using System;

public class BadEffectUI : EffectUIButton
{ 
    private EffectsManager.NegativeEffects _negativeEffect;
    
    protected override void EffectSetup()
    {
        base.EffectSetup();
        
        TypeOfEffect = EffectsManager.TypeOfCharEffect.BadEffect;
        
        _negativeEffect = (EffectsManager.NegativeEffects)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EffectsManager.NegativeEffects)).Length);
        
        IconImage.sprite = _negativeEffect switch
        {
            EffectsManager.NegativeEffects.FasterCharacters => EffectsManager.Instance.HighSpeedIcon,
            EffectsManager.NegativeEffects.Rain => EffectsManager.Instance.RainIcon,
            EffectsManager.NegativeEffects.ShrinkCharacters => EffectsManager.Instance.ShrinkCharacterIcon,
            _ => throw new ArgumentOutOfRangeException()
        };
        EffectTime = _negativeEffect switch
        {
            EffectsManager.NegativeEffects.FasterCharacters => EffectsManager.Instance.SpeedEffectDuration,
            EffectsManager.NegativeEffects.Rain => EffectsManager.Instance.RainEffectDuration,
            EffectsManager.NegativeEffects.ShrinkCharacters => EffectsManager.Instance.ShrinkEffectDuration,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        EffectsManager.OnEffectGiven -= EffectSetup;
        
        ExecuteEffect();
    }
    
    protected override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Action effect = _negativeEffect switch
        {
            EffectsManager.NegativeEffects.FasterCharacters => EffectsManager.Instance.FasterCharacters,
            EffectsManager.NegativeEffects.Rain => EffectsManager.Instance.StartRain,
            EffectsManager.NegativeEffects.ShrinkCharacters => EffectsManager.Instance.ShrinkCharacters,
            _ => throw new ArgumentOutOfRangeException()
        };
        effect.Invoke();
    }
}
