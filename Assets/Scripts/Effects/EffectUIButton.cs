using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectUIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform SlotTransform { get; set; }
    
    protected Image IconImage;
    protected float EffectTime;
    
    protected EffectsManager.TypeOfCharEffect TypeOfEffect;
    
    private void OnEnable()
    {
        EffectsManager.OnEffectGiven += EffectSetup;
        EffectsManager.OnEffectDone += UpdatePosition;
    }
    private void OnDisable()
    {
        EffectsManager.OnEffectDone -= UpdatePosition;
    }
    
    protected virtual void EffectSetup()
    {
        IconImage = transform.GetChild(0).GetComponent<Image>();
        
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.DOMove(SlotTransform.position, 0.5f).SetEase(Ease.OutBack);
    }

    private Sequence _useSequence;
    protected virtual void ExecuteEffect()
    {
        _usedEffect = true;
        
        _scaleTween?.Kill();
        _useSequence = DOTween.Sequence();
        
        _useSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -30), 0.2f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));
        _useSequence.Join(DOTween.To(() => 1f, h => IconImage.fillAmount = h, 0f, EffectTime).SetEase(Ease.Linear));
        _useSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, 360), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(3, LoopType.Restart));
        _useSequence.Join(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)).OnComplete(()=>
        {
            Destroy(gameObject);
            Destroy(SlotTransform.gameObject);
            DOVirtual.DelayedCall( 0.1f, () =>
            {
                EffectsManager.Instance.TriggerOnEffectDone();
            });
        });
    }

    private bool _usedEffect;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(TypeOfEffect == EffectsManager.TypeOfCharEffect.GoodEffect && !_usedEffect) ExecuteEffect();
    }

    private Tween _scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    { 
        if(_usedEffect) return;
        
        _scaleTween?.Kill();
        
        _scaleTween = transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        if(_usedEffect) return;
        
        _scaleTween?.Kill();
        
        _scaleTween = transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
}
