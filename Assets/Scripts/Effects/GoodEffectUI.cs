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
            EffectsManager.PositiveEffects.GrowSearchable => EffectsManager.Instance.GrowSearchableIcon,
            _ => throw new ArgumentOutOfRangeException()
        };

        EffectTime = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.CameraEffectDuration,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.StopEffectDuration,
            EffectsManager.PositiveEffects.GrowSearchable => EffectsManager.Instance.GrowEffectDuration,
            _ => throw new ArgumentOutOfRangeException()
        };

        DescriptionText.text = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.BetterCameraDescription,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.FreezeEffectDescription,
            EffectsManager.PositiveEffects.GrowSearchable => EffectsManager.Instance.GrowEffectDescription,
            _ => throw new ArgumentOutOfRangeException()
        };

        NameText.text = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.BetterCameraEffectName,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.FreezeEffectName,
            EffectsManager.PositiveEffects.GrowSearchable => EffectsManager.Instance.GrowEffectName,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        EffectsManager.OnEffectGiven -= EffectSetup;
    }
    public static int GoodEffect { get; set; }
    protected override void ExecuteEffect()
    {
        base.ExecuteEffect();
        
        AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[5]);
        
        Action effect = _goodEffect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.BetterCamera,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.StopCharacterMovement,
            EffectsManager.PositiveEffects.GrowSearchable => EffectsManager.Instance.GrowSearchable,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        effect.Invoke();
        GoodEffect++;
    }
}
