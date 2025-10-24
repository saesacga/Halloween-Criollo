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
            _ => throw new ArgumentOutOfRangeException()
        };
        EffectTime = _negativeEffect switch
        {
            EffectsManager.NegativeEffects.FasterCharacters => EffectsManager.Instance.SpeedEffectDuration,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        ExecuteEffect();
    }
    
    protected override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Action effect = _negativeEffect switch
        {
            EffectsManager.NegativeEffects.FasterCharacters => EffectsManager.Instance.FasterCharacters,
            _ => throw new ArgumentOutOfRangeException()
        };
        effect.Invoke();
    }
}
