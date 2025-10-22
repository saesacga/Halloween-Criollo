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
    
    private TextMeshProUGUI _levelCompletedText;
    private TextMeshProUGUI _charactersFoundedNumberText;
    private TextMeshProUGUI _clickToContinueText;

    private Sequence _sequence;
    private bool _sequenceEnd;
    public static event Action OnEndLevelUIOpen;

    private void Start()
    {
        _levelCompletedText = _levelCompleted.GetComponent<TextMeshProUGUI>();
        _charactersFoundedNumberText = _charactersFoundedNumber.GetComponent<TextMeshProUGUI>();
        _clickToContinueText = _clickToContinue.GetComponent<TextMeshProUGUI>();
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
        _sequence.timeScale = 2f;
        _sequence.PlayBackwards();
        _sequenceEnd = false;
        //OnEndLevelUIOpen?.Invoke();
        Debug.Log("Clicked");

    }

    public void SetLevelCompletedText()
    {
        _sequence = DOTween.Sequence();
        _sequence.SetAutoKill(false);
        
        int displayedValue = 0;
        float currentValue = 0;

        var counterTween = DOTween.To(() => currentValue, x => currentValue = x, GameManager.Instance.DailyScore, 3)
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
        
        _levelCompleted.DORotate(new Vector3(0, 10, 5), _animationDuration / 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        DOTween.To(() => 0f, h => _levelCompletedText.color = Color.HSVToRGB(h, 1f, 1f), 1f, 3f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        
        _sequence.Append(_bg.DOAnchorPos(Vector2.zero, _animationDuration).SetEase(Ease.OutBounce));
        _sequence.Append(_levelCompleted.DOScale(Vector3.one, _animationDuration/2).SetEase(Ease.OutBack));
        _sequence.Append(_charactersFounded.DOAnchorPosY(1, _animationDuration/2).SetEase(Ease.OutBack));
        _sequence.AppendInterval(0.2f);
        _sequence.Append(counterTween);
        _sequence.Append(_charactersFoundedNumber.DOPunchScale(Vector3.one * 0.6f, 0.1f, 4)).OnComplete(() =>
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
}