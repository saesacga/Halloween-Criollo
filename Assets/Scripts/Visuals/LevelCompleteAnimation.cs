using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

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
    [SerializeField] private RectTransform _missingCharactersMessage;
    
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
    public static event Action OnEndLevelUIClose;

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
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_sequenceEnd) return;

        if (GameManager.Instance.CurrentLevel == GameManager.Level.Chaos  && GameManager.Instance.ChaosNight == 1)
        {
            StatsMenu1.Instance.ShowStats();
            GameManager.Instance.TotalMistakes = 0;
            GameManager.Instance.TotalScore = 0;
            _sequenceEnd = false;
            return;
        }
        CloseLevelCompleteUI();
        _sequenceEnd = false;
    }

    private Tween _levelCompletedRotationTween;
    private Tween _levelCompletedTextTween;
    private void SetEndMessage()
    {
        _levelCompletedRotationTween?.Kill();
        _levelCompleted.DORotate(new Vector3(0, -10, -5), 0.01f);
        _levelCompletedTextTween?.Kill();
        
        _levelCompletedRotationTween = _levelCompleted.DORotate(new Vector3(0, 10, 5), _animationDuration / 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        
        if (GameManager.Instance.DailyScore < 11)
        {
            _levelCompletedText.text = "Mala Suerte";
            _levelCompletedText.color = Color.crimson;
        }
        else
        {
            _levelCompletedText.text = GameManager.Instance.CurrentLevel == GameManager.Level.Three ? "Juego Completado!" : "Noche Completada";
            _levelCompletedTextTween = DOTween.To(() => 0f, h => _levelCompletedText.color = Color.HSVToRGB(h, 1f, 1f), 1f, 3f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }

    private Tween _charactersFoundedTween;
    private Tween _clickToContinueTween;
    private Tween _missingCharactersMessageTween;
    [Button]
    public void OpenLevelCompleteUI()
    {
        _charactersFoundedTween?.Kill();
        _charactersFoundedNumber.localScale = Vector3.one;
        _clickToContinueTween?.Kill();
        _clickToContinueText.alpha = 1f;
        _charactersFoundedNumberText.text = "0";
        _charactersFoundedNumberText.color = Color.gray6;
        
        if(_openSequence!=null && _openSequence.IsActive() && !_openSequence.IsPlaying()) _openSequence.Kill();
        
        _openSequence = DOTween.Sequence();

        SetEndMessage();
        
        _openSequence.Append(_bg.DOAnchorPos(Vector2.zero, _animationDuration).SetEase(Ease.OutBounce).OnComplete(()=>OnEndLevelUIOpen?.Invoke()));
        _openSequence.Append(_levelCompleted.DOScale(Vector3.one, _animationDuration/2).SetEase(Ease.OutBack));
        _openSequence.Append(_charactersFounded.DOAnchorPosY(1, _animationDuration/2).SetEase(Ease.OutBack));
        _openSequence.Append(CreateCounterTween(_charactersFoundedNumberText, GameManager.Instance.DailyScore)
            .OnComplete(() =>
            {
                if (GameManager.Instance.DailyScore < 11)
                {
                    _charactersFoundedNumberText.color = Color.crimson;
                    _missingCharactersMessage.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        _missingCharactersMessageTween = _missingCharactersMessage.DOScale(Vector3.one * 1.2f, 0.4f)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
                }
                else
                {
                    _charactersFoundedNumberText.color = Color.lawnGreen;
                }
                _charactersFoundedTween = _charactersFoundedNumber.DOScale(Vector3.one * 1.6f, 0.6f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
                _clickToContinueTween = _clickToContinueText.DOFade(0.3f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }));
        _openSequence.Append(_clickToContinue.DOAnchorPosY(-438, 0.2f).SetEase(Ease.OutBack).SetDelay(1f)).OnComplete(()=>_sequenceEnd = true);
    }

    public static Tween CreateCounterTween(TMP_Text text, float targetValue)
    {
        var displayedValue = 0;
        var currentValue = 0f;
        
        var minDuration = 0.2f;
        var maxDuration = 3f;
        var maxReferenceValue = 10f;
        var punchScale = 0.6f;
        var punchDuration = 0.2f;

        float duration = Mathf.Lerp(minDuration, maxDuration, Mathf.Clamp01(targetValue / maxReferenceValue));

        var tween = DOTween.To(() => currentValue, x => currentValue = x, targetValue, duration)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                int newValue = Mathf.FloorToInt(currentValue);
                if (newValue == displayedValue) return;

                AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[8]);
                displayedValue = newValue;
                text.text = displayedValue.ToString();
                
                text.transform.DOKill(true);
                text.transform.localScale = Vector3.one;
                text.transform.DOPunchScale(Vector3.one * punchScale, punchDuration, 4);
            });

        return tween;
    }
    
    public void CloseLevelCompleteUI()
    {
        if(_closeSequence!=null && _closeSequence.IsActive() && !_closeSequence.IsPlaying()) _closeSequence.Kill();
        
        _missingCharactersMessageTween?.Kill();
        
        _closeSequence = DOTween.Sequence();
        
        _closeSequence.Append(_clickToContinue.DOAnchorPosY(_clickToContinueInitialPos, 0.2f).SetEase(Ease.InBack));
        _closeSequence.Join(_missingCharactersMessage.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
        _closeSequence.Append(_charactersFounded.DOAnchorPosY(_charactersFoundedInitialPos, _animationDuration/2).SetEase(Ease.InBack));
        _closeSequence.Append(_levelCompleted.DOScale(Vector3.zero, _animationDuration/2).SetEase(Ease.InBack).OnComplete(()=>OnEndLevelUIClose?.Invoke()));
        _closeSequence.Append(_bg.DOAnchorPosY(_bgInitialPos, _animationDuration).SetEase(Ease.InBounce));
    }
}