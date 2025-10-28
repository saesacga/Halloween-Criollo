using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ShowChaosRules : MonoBehaviour, IPointerClickHandler, IUIAnimated
{
    [TabGroup("Chaos Related"), DisableIf("@_chaosContainer != null"),SerializeField]
    private RectTransform _chaosContainer;
    private float _chaosContainerInitialYPos;
    [TabGroup("Chaos Related"), SerializeField]
    private TextMeshProUGUI _chaosRulesTitle;
    
    [SerializeField] private TextMeshProUGUI _clickToContinueText;
    
    private Sequence _sequence;
    private bool _completeChaosSequence;
    
    private RectTransform _thisRect;
    
    private void Start()
    {
        _chaosContainerInitialYPos = _chaosContainer.localPosition.y;
        
        _thisRect = GetComponent<RectTransform>();
    }
    
    [Button]
    public void ChaosRules()
    { 
        if (_sequence != null && _sequence.IsActive() && _sequence.IsPlaying()) return;
        
        OnAnimationStart?.Invoke();
        _clickToContinueText.transform.localScale = Vector3.zero;
        _clickToContinueText.alpha = 1f;
        
        DOTween.To(() => 0f, h => _chaosRulesTitle.color = Color.HSVToRGB(h, 1f, 1f), 1f, 3f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        
        _sequence = DOTween.Sequence();
        
        _sequence.Append(_chaosContainer.DOLocalMoveY(0, 1f).SetEase(Ease.OutBounce));
        _sequence.Append(_clickToContinueText.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                _clickToContinueText.GetComponent<TextMeshProUGUI>().DOFade(0.3f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
                _completeChaosSequence = true;
            });
    }
    private void HideChaosRules()
    {
        if (_sequence != null && _sequence.IsActive() && _sequence.IsPlaying()) return;
        
        _chaosContainer.DOLocalMoveY(_chaosContainerInitialYPos, 1f).SetEase(Ease.InBounce);
        _clickToContinueText.transform.DOScale(0f, 1f).SetEase(Ease.OutBack);
        OnAnimationEnd?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_completeChaosSequence)
        {
            HideChaosRules();
        }
    }
    
    public event Action OnAnimationStart;
    public event Action OnAnimationEnd;
}
