using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class StatsMenu1 : MonoBehaviour, IPointerClickHandler, IUIAnimated
{
    [SerializeField, TabGroup("Stats Related")] 
    private GameObject _statEntryPrefab;
    [SerializeField, TabGroup("Stats Related")]
    private RectTransform _statsLayoutGroup;
    
    [SerializeField, DisableIf("@_clickToContinueText != null")] 
    private TextMeshProUGUI _clickToContinueText;
    [SerializeField, DisableIf("@_containerRect != null")] private RectTransform _containerRect;
    private float _containerInitialYPos;
    
    private List<(string, int)> _finishGameStats;
    private List<(string, int)> _chaosStats;
    
    private void Start()
    {
        _containerInitialYPos = _containerRect.anchoredPosition.y;
    }
    
    private Sequence _sequence;
    private bool _completeSequence;
    
    private void ShowStats(List<(string label, int value)> stats, float animationDuration = 1f)
    {
        _sequence?.Kill();
        
        foreach (Transform child in _statsLayoutGroup)
            Destroy(child.gameObject);

        List<TMP_Text> statNumbers = new List<TMP_Text>();
        
        foreach (var element in stats)
        {
            var newStat = Instantiate(_statEntryPrefab, _statsLayoutGroup);
            var texts = newStat.GetComponentsInChildren<TMP_Text>();
            texts[0].text = element.label;
            texts[1].text = "0";
            statNumbers.Add(texts[1]);
        }

        _sequence = DOTween.Sequence();
        OnAnimationStart?.Invoke();
        
        _sequence.Append(_containerRect.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack));
        
        for (int i = 0; i < stats.Count; i++)
        {
            int targetValue = stats[i].value;
            TMP_Text numberText = statNumbers[i];
            _sequence.Append(LevelCompleteAnimation.CreateCounterTween(numberText, targetValue));
            _sequence.AppendInterval(0.1f);
        }

        _sequence.Append(_clickToContinueText.transform.DOScale(1f, 1f).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                _clickToContinueText.GetComponent<TextMeshProUGUI>().DOFade(0.3f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
                _completeSequence = true;
            });
    }

    private void HideStats()
    {
        if (_sequence != null && _sequence.IsActive() && _sequence.IsPlaying()) return;
        
        _sequence?.Kill();
        
        _sequence = DOTween.Sequence();
        
        _sequence.Append(_containerRect.DOAnchorPosY(_containerInitialYPos, 1f).SetEase(Ease.InBounce));
        _sequence.Append(_clickToContinueText.transform.DOScale(0f, 1f).SetEase(Ease.InBack));
        
        OnAnimationEnd?.Invoke();
        
        _completeSequence = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_completeSequence) return;
        
        HideStats();
    }

    [Button]
    public void ShowFinishGameStats()
    {
        _finishGameStats = new List<(string, int)>
        {
            ("Disfraces encontrados", GameManager.Instance.TotalScore),
            ("Errores totales", GameManager.Instance.TotalMistakes),
            ("Intentos totales hasta pasar la noche del 31", GameManager.Instance.TotalTries)
        };
        
        ShowStats(_finishGameStats);
    }
    
    [Button]
    public void ShowChaosStats()
    {
        _chaosStats = new List<(string, int)>
        {
            ("Disfraces encontrados en modo Caos", GameManager.Instance.TotalScore),
            ("Errores totales en modo Caos", GameManager.Instance.TotalMistakes),
            ("Efectos positivos usados", GoodEffectUI.GoodEffect),
            ("Efectos negativos activados", BadEffectUI.BadEffectsCount),
            ("Noches en modo Caos completadas", GameManager.Instance.ChaosNight - 1)
        };
        
        ShowStats(_chaosStats);
    }
    
    public event Action OnAnimationStart;
    public event Action OnAnimationEnd;
}
