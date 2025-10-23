using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PowerUpCharacter : Character
{
    [SerializeField, EnumToggleButtons] private EffectsManager.TypeOfCharEffect _typeOfCharEffect;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        SetSearchable();
        DOVirtual.DelayedCall( 10, () =>
        {
            EffectsManager.Instance.GiveEffectToPlayer(transform, _typeOfCharEffect);
        });
    }
    
    protected override void SetSearchableUI()
    {
        Debug.Log("Someone enter to help");
    }

    protected override void SetUnsearchableUI()
    {
        if (_typeOfCharEffect == EffectsManager.TypeOfCharEffect.GoodEffect)
        {
            EffectsManager.Instance.GiveEffectToPlayer(transform, _typeOfCharEffect);
        }
    }
}
