using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

public class LevelCompleteAnimation : MonoBehaviour, IPointerClickHandler
{
    #region Singleton
    public static LevelCompleteAnimation Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
    }

    #endregion

    #region Animation Params
    
    [SerializeField] private float _animationDuration = 1f;

    #endregion
    
    #region UI Scene References
    
    [SerializeField] private RectTransform _levelCompleted;
    
    [SerializeField] private RectTransform _charactersFounded;
   
    [SerializeField] private RectTransform _charactersFoundedNumber;
    
    [SerializeField] private RectTransform _clickToContinue;
    
    [SerializeField] private RectTransform _bg;
    
    #endregion
    
    #region Save Initial Transforms
    
    private float _clickToContinueInitialPos;
    private float _charactersFoundedInitialPos;
    private float _bgInitialPos;
    
    #endregion
    
    private TextMeshProUGUI _levelCompletedText;
    private TextMeshProUGUI _charactersFoundedNumberText;
    private TextMeshProUGUI _clickToContinueText;

    private Sequence _openSequence;
    private Sequence _closeSequence;
    private bool _sequenceEnd;
    public static event Action OnEndLevelUIOpen;

    private void Start()
    {
        _levelCompletedText = _levelCompleted.GetComponent<TextMeshProUGUI>();
        _charactersFoundedNumberText = _charactersFoundedNumber.GetComponent<TextMeshProUGUI>();
        _clickToContinueText = _clickToContinue.GetComponent<TextMeshProUGUI>();

        #region Save Transforms

        _clickToContinueInitialPos = _clickToContinue.anchoredPosition.y;
        _charactersFoundedInitialPos = _charactersFounded.anchoredPosition.y;
        _bgInitialPos = _bg.anchoredPosition.y;

        #endregion
    }

    private void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            SetLevelCompletedText();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_sequenceEnd) return;
        
        CloseLevelCompleteUI();
        _sequenceEnd = false;
        OnEndLevelUIOpen?.Invoke();
        Debug.Log("Clicked");

    }

    public void SetLevelCompletedText()
    {
        _openSequence = DOTween.Sequence();
        
        _levelCompleted.DORotate(new Vector3(0, 10, 5), _animationDuration / 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        DOTween.To(() => 0f, h => _levelCompletedText.color = Color.HSVToRGB(h, 1f, 1f), 1f, 3f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        
        _openSequence.Append(_bg.DOAnchorPos(Vector2.zero, _animationDuration).SetEase(Ease.OutBounce));
        _openSequence.Append(_levelCompleted.DOScale(Vector3.one, _animationDuration/2).SetEase(Ease.OutBack));
        _openSequence.Append(_charactersFounded.DOAnchorPosY(1, _animationDuration/2).SetEase(Ease.OutBack));
        _openSequence.AppendInterval(0.2f);
        _openSequence.Append(CounterTween());
        _openSequence.Append(_charactersFoundedNumber.DOPunchScale(Vector3.one * 0.6f, 0.1f, 4)).OnComplete(() =>
        {
            _charactersFoundedNumber.DOScale(Vector3.one * 1.6f, 0.6f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            _clickToContinue.DOAnchorPosY(-438, 0.2f).SetEase(Ease.OutBack).SetDelay(1f);
            _clickToContinueText.DOFade(0.3f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            
            _sequenceEnd = true;
        });
    }

    private Tween CounterTween()
    {
        int displayedValue = 0;
        float currentValue = 0;
        
        var counterTween = DOTween.To(() => currentValue, x => currentValue = x, GameManager.Instance.DailyScore, 2)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                int newValue = Mathf.FloorToInt(currentValue);
                if (newValue == displayedValue) return;
                displayedValue = newValue;
                _charactersFoundedNumberText.text = displayedValue.ToString();

                _charactersFoundedNumber.DOKill(true);
                _charactersFoundedNumber.localScale = Vector3.one;
                _charactersFoundedNumber.DOPunchScale(Vector3.one * 0.6f, 0.2f, 4);
            });
        
        return counterTween;
    }

    private void CloseLevelCompleteUI()
    {
        _closeSequence = DOTween.Sequence();
        
        _closeSequence.Append(_clickToContinue.DOAnchorPosY(_clickToContinueInitialPos, 0.2f).SetEase(Ease.InBack));
        _closeSequence.Append(_charactersFounded.DOAnchorPosY(_charactersFoundedInitialPos, _animationDuration/2).SetEase(Ease.InBack));
        _closeSequence.Append(_levelCompleted.DOScale(Vector3.zero, _animationDuration/2).SetEase(Ease.InBack));
        _closeSequence.Append(_bg.DOAnchorPosY(_bgInitialPos, _animationDuration).SetEase(Ease.InBounce));
        _closeSequence.OnComplete(() =>
        {
            _levelCompleted.DOKill();
            _levelCompleted.DORotate(new Vector3(0, -10, -5), 0.01f);
            _charactersFoundedNumberText.text = null;
        });
    }
}