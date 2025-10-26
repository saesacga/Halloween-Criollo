using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Stats2 : MonoBehaviour, IPointerClickHandler
{
    #region Singleton 
    public static Stats2 Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion
    
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat1;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat2;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat3;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat4;
    [TabGroup("StatsReferences"), SerializeField] 
    private TextMeshProUGUI _stat5;
    private RectTransform _thisTransform;

    private void OnEnable()
    {
        _thisTransform = GetComponent<RectTransform>();
    }
    
    private bool _completeSequence;
    private Sequence _sequence;
    [Button]
    public void ShowStats2()
    {
        _sequence?.Kill();
        
        _sequence = DOTween.Sequence();

        _sequence.Append(_thisTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutBounce));
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat1, GameManager.Instance.TotalScore)); //Total Score Caos
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat2, GameManager.Instance.TotalMistakes)); //Total Mistakes Caos
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat3, GoodEffectUI.GoodEffect)); //Total Effects Caos
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat4, BadEffectUI.BadEffectsCount));//Total NegEffects Caos
        _sequence.AppendInterval(0.1f);
        _sequence.Append(LevelCompleteAnimation.CreateCounterTween(_stat5, GameManager.Instance.ChaosNight-1));//Total Nights Caos
        _sequence.AppendInterval(1f).OnComplete(() =>
            {
                _completeSequence = true;
            });
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_completeSequence) return;
        
        _sequence?.Kill();
            
        _sequence = DOTween.Sequence();
            
        LevelCompleteAnimation.Instance.CloseLevelCompleteUI();
        _sequence.Append(_thisTransform.DOAnchorPosY(1304, 1f).SetEase(Ease.OutBounce));
    }
}
