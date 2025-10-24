using UnityEngine;
using System;

public class GoodEffectUI : EffectUIButton
{
    private EffectsManager.PositiveEffects _goodEffect;

    protected override void EffectSetup()
    {
        base.EffectSetup();
        
        TypeOfEffect = EffectsManager.TypeOfCharEffect.GoodEffect;
        
        _goodEffect = (EffectsManager.PositiveEffects)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EffectsManager.PositiveEffects)).Length);
        
        IconImage.sprite = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.BetterCameraIcon,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.StopCharacterMoveIcon,
            _ => throw new ArgumentOutOfRangeException()
        };

        EffectTime = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.CameraEffectDuration,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.StopEffectDuration,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Action effect = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.BetterCamera,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.StopCharacterMovement,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        effect.Invoke();
    }
}
