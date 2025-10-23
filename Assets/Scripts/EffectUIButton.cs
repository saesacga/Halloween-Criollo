using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectUIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform SlotTransform { get; set; }
    
    private EffectsManager.PositiveEffects _effect;
    private Image _image;
    
    private void SlotValueChanged()
    {
        transform.DOMove(SlotTransform.position, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnEnable()
    {
        EffectsManager.OnEffectGiven += SlotValueChanged;
        
        _effect = (EffectsManager.PositiveEffects)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EffectsManager.PositiveEffects)).Length);
        _image = transform.GetChild(0).GetComponent<Image>();

        _image.sprite = _effect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.HighSpeedIcon,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.BetterCameraIcon,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void ExecuteEffect()
    {
        Action effect = _effect switch
        {
            EffectsManager.PositiveEffects.BetterCamera => EffectsManager.Instance.BetterCamera,
            EffectsManager.PositiveEffects.StopCharacterMovement => EffectsManager.Instance.StopCharacterMovement,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        effect.Invoke();
    }
    
    private Tween _useTween;
    public void OnPointerClick(PointerEventData eventData)
    {
        ExecuteEffect();
        
        if (_useTween != null && _useTween.IsPlaying()) return;
        
        _useTween = transform.DOLocalRotate(new Vector3(0, 0, -30), 0.2f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    { 
        transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
}
