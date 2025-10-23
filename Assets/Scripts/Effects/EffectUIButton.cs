using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectUIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform SlotTransform { get; set; }
    
    protected Image IconImage;
    
    protected EffectsManager.TypeOfCharEffect TypeOfEffect;
    
    protected virtual void OnEnable()
    {
        EffectsManager.OnEffectGiven += EffectSetup;
    }
    
    protected virtual void EffectSetup()
    {
        IconImage = transform.GetChild(0).GetComponent<Image>();
        
        transform.DOMove(SlotTransform.position, 0.5f).SetEase(Ease.OutBack);
    }

    protected virtual void ExecuteEffect()
    {
        if (_useTween != null && _useTween.IsPlaying()) return;
        
        _useTween = transform.DOLocalRotate(new Vector3(0, 0, -30), 0.2f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
    }
    
    private Tween _useTween;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(TypeOfEffect == EffectsManager.TypeOfCharEffect.GoodEffect) ExecuteEffect();
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
