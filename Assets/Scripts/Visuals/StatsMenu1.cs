using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class StatsMenu1 : MonoBehaviour, IPointerClickHandler
{
    #region Singleton 
    public static StatsMenu1 Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion
    
    [TabGroup("ChaosRules"), SerializeField]
    private TextMeshProUGUI _chaosRulesText;
    [TabGroup("ChaosRules"), SerializeField]
    private TextMeshProUGUI _chaosTitleText;
    
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat1;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat2;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat3;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _clickToContinueText;
    [TabGroup("StatsReferences"), SerializeField]
    private RectTransform _statsContainer;
    private RectTransform _parent;

    private void OnEnable()
    {
        _parent = GetComponent<RectTransform>();
    }
    
    private bool _completeSequence;
    private Sequence _sequence;
    
    [Button]
    public void ShowStats()
    {
        _sequence?.Kill();
        _statsContainer.DOAnchorPosY(77, 0.01f);
        
        _sequence = DOTween.Sequence();

        _sequence.Append(_parent.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutBounce));
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat1, GameManager.Instance.TotalScore));
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat2, GameManager.Instance.TotalMistakes));
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat3, GameManager.Instance.TotalTries));
        _sequence.AppendInterval(1f);
        _sequence.Append(_clickToContinueText.transform.DOScale(1f, 1f).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                _completeSequence = true;
                _clickToContinueText.DOFade(0.3f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }

    private bool _completeSequence2;
    private void ShowChaosRules()
    {
        if (_sequence != null && _sequence.IsPlaying()) return;
        
        _sequence?.Kill();
        
        _clickToContinueText.transform.DOScale(0, 1f).SetEase(Ease.OutBack);
        _clickToContinueText.DOFade(1f, 1f);
        
        _sequence = DOTween.Sequence();
        
        DOTween.To(() => 0f, h => _chaosTitleText.color = Color.HSVToRGB(h, 1f, 1f), 1f, 3f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        
        _sequence.Append(_statsContainer.DOAnchorPosY(1304f, 0.2f).SetEase(Ease.InBack));
        _sequence.AppendInterval(1f);
        _sequence.Append(_chaosRulesText.DOFade(1f, 1f));
        _sequence.Join(_chaosTitleText.DOFade(1f, 1f)); 
        _sequence.AppendInterval(3f);
        _sequence.Append(_clickToContinueText.transform.DOScale(1f, 1f).SetEase(Ease.OutBack)).OnComplete(()=>
        {
            _completeSequence2 = true;
            _clickToContinueText.DOFade(0.3f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_completeSequence)
        {
            ShowChaosRules();
        }

        if (!_completeSequence2) return;
        
        _sequence?.Kill();
            
        _sequence = DOTween.Sequence();
            
        LevelCompleteAnimation.Instance.CloseLevelCompleteUI();
        _sequence.Append(_parent.DOAnchorPosY(1304, 1f).SetEase(Ease.OutBounce));
    }
}
