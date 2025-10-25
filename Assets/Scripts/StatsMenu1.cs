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
    
    [SerializeField] private TextMeshProUGUI _stat1;
    [SerializeField] private TextMeshProUGUI _stat2;
    [SerializeField] private TextMeshProUGUI _stat3;
    [SerializeField] private TextMeshProUGUI _clickToContinueText;
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
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_completeSequence)
        {
            _sequence?.Kill();
            
            _sequence = DOTween.Sequence();
            
            LevelCompleteAnimation.Instance.CloseLevelCompleteUI();
            _sequence.Append(_parent.DOAnchorPosY(1304, 1f).SetEase(Ease.OutBounce));
        }
    }
}
