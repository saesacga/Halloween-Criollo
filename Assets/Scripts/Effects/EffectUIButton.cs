using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectUIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform SlotTransform { get; set; }
    
    protected float EffectTime;
    
    protected EffectsManager.TypeOfCharEffect TypeOfEffect;
    
    [SerializeField]
    protected Image IconImage;
    [SerializeField]
    protected TextMeshProUGUI DescriptionText;
    [SerializeField]
    protected TextMeshProUGUI NameText;
    
    private void OnEnable()
    {
        EffectsManager.OnEffectGiven += EffectSetup;
        EffectsManager.OnEffectDone += UpdatePosition;
        
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
    private void OnDisable()
    {
        EffectsManager.OnEffectDone -= UpdatePosition;
    }
    
    protected virtual void EffectSetup()
    {
        UpdatePosition();
    }

    private bool _inSlotPosition;
    private void UpdatePosition()
    {
        transform.DOMove(SlotTransform.position, 0.5f).SetEase(Ease.OutBack).OnComplete(()=>_inSlotPosition = true);
    }

    private Sequence _useSequence;
    protected virtual void ExecuteEffect()
    {
        _usedEffect = true;
        
        _scaleSeq?.Kill();
        _scaleSeq = DOTween.Sequence();
        
        _scaleSeq.Append(transform.DOScale(Vector3.one, 0.2f));
        _scaleSeq.Append(DescriptionText.DOFade(0f, 0.2f));
        _scaleSeq.Join(NameText.DOFade(0f, 0.2f)).SetEase(Ease.OutBack);
        
        _useSequence = DOTween.Sequence();
        
        _useSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -30), 0.2f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));
        _useSequence.Join(DOTween.To(() => 1f, h => IconImage.fillAmount = h, 0f, EffectTime).SetEase(Ease.Linear)
            .OnComplete(() => AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[4], 2f)));
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
        if (!_inSlotPosition) return;
        if(TypeOfEffect == EffectsManager.TypeOfCharEffect.GoodEffect && !_usedEffect) ExecuteEffect();
    }

    private Sequence _scaleSeq;
    public void OnPointerEnter(PointerEventData eventData)
    { 
        if (!_inSlotPosition) return;
        
        _scaleSeq?.Kill();
        _scaleSeq = DOTween.Sequence();
        
        _scaleSeq.Append(DescriptionText.DOFade(1f, 0.2f));
        _scaleSeq.Join(NameText.DOFade(1f, 0.2f)).SetEase(Ease.OutBack);
        
        if(_usedEffect) return;
        
        _scaleSeq.Join(transform.DOScale(Vector3.one * 1.2f, 0.2f));
        
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        if (!_inSlotPosition) return;
        
        _scaleSeq?.Kill();
        _scaleSeq = DOTween.Sequence();
        
        _scaleSeq.Append(DescriptionText.DOFade(0f, 0.2f));
        _scaleSeq.Join(NameText.DOFade(0f, 0.2f)).SetEase(Ease.OutBack);
        
        if(_usedEffect) return;
        
        _scaleSeq.Join(transform.DOScale(Vector3.one, 0.2f));
    }
}
