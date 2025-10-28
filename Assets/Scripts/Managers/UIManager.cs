using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    #region Touch Controls

    [SerializeField, TabGroup("Touch", TextColor = "yellow")]
    private GameObject _touchControls;
    
    #endregion

    #region Singleton

    public static UIManager Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
        
        _containerInitialYPos = _containerRect.anchoredPosition.y;
    }

    #endregion

    #region Pause Menu

    private bool _isPaused;
    
    [TabGroup("⏸", TextColor = "yellow"), SerializeField] private RectTransform _containerRect;
    [TabGroup("⏸"), SerializeField] private RectTransform _pauseTextTransform;
    private float _containerInitialYPos;
    
    private void Update()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
        
        TogglePauseMenu();
    }
    
    public void TogglePauseMenu()
    {
        if (_isPaused)
            ResumeGame();
            
        else 
            PauseGame();
    }
    
    private Tween _pauseTween;
    private Tween _pauseTextTween;
    private void PauseGame()
    {
        _pauseTween?.Kill();
        _pauseTextTween?.Kill();
        
        AudioManager.Instance.PauseAllAudio();
        Time.timeScale = 0f;
        
        _pauseTween = _containerRect.DOLocalMoveY(0, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
        _pauseTextTween = _pauseTextTransform.DORotate(new Vector3(0, 10, 5), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        
        _isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.ResumeAllAudio();
        
        _isPaused = false;
        _pauseTween?.Kill();
        _pauseTextTween?.Kill();
        
        _pauseTween = _containerRect.DOAnchorPosY(_containerInitialYPos, 0.2f).SetEase(Ease.OutBack);
        _pauseTextTween = _pauseTextTransform.DORotate(new Vector3(0, -10, -5), 0.2f).SetEase(Ease.OutBack);
    }

    #endregion

    #region Sequences
    
    [TabGroup("Sequences", TextColor = "yellow"), SerializeField]
    private LevelCompleteAnimation _levelCompleteAnimation;
    [TabGroup("Sequences"), SerializeField]
    private StatsMenu1 _statsMenu1;
    [TabGroup("Sequences"), SerializeField]
    private ShowChaosRules _chaosRules;
    
    private void Start()
    {
        _levelCompleteAnimation.OnAnimationStart += OnLevelCompleteStart;
        _levelCompleteAnimation.OnAnimationEnd += OnLevelCompleteEnd;
        _statsMenu1.OnAnimationEnd += OnStatsMenuEnd;
        _chaosRules.OnAnimationEnd += OnChaosRulesEnd;
    }

    public void StartSequence()
    {
        _levelCompleteAnimation.OpenLevelCompleteUI();
    }
    
    public static event Action OnEndLevelUIOpen; 
    private void OnLevelCompleteStart()
    {
        OnEndLevelUIOpen?.Invoke();
        GameManager.Instance.NewLevelOpenMenuConfig();
    }
    
    public static event Action OnEndLevelUIClose; 
    private bool _showFinishGameStats;
    private bool _achievedChaos;
    private void OnLevelCompleteEnd()
    {
        OnEndLevelUIClose?.Invoke();
        GameManager.Instance.NewLevelCloseMenuConfig();
        switch (GameManager.Instance.CurrentLevel)
        {
            case GameManager.Level.Chaos when !_showFinishGameStats:
                _statsMenu1.ShowFinishGameStats();
                _showFinishGameStats = true;
                _achievedChaos = true;
                break;
            case GameManager.Level.One when _achievedChaos:
                _statsMenu1.ShowChaosStats();
                _showFinishGameStats = false;
                _achievedChaos = false;
                break;
            default:
                GameTime.Instance.SetTime();
                break;
        }
    }

    private void OnStatsMenuEnd()
    {
        if (GameManager.Instance.CurrentLevel == GameManager.Level.Chaos)
        {
            _chaosRules.ChaosRules();
            
            GameManager.Instance.TotalScore = 0;
            GameManager.Instance.TotalMistakes = 0;
            GameManager.Instance.TotalTries = 1;
            
            return;
        }
        GameTime.Instance.SetTime();
        OnEndLevelUIClose?.Invoke();
        
        GameManager.Instance.TotalScore = 0;
        GameManager.Instance.TotalMistakes = 0;
        BadEffectUI.BadEffectsCount = 0;
        GoodEffectUI.GoodEffect = 0;
        GameManager.Instance.ChaosNight = 1;
    }

    private void OnChaosRulesEnd()
    {
        GameTime.Instance.SetTime();
        OnEndLevelUIClose?.Invoke();
    }
    
    #endregion
}
