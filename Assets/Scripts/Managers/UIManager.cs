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

        #region TouchControls
        
        var isTouch = Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
        
        _touchControls.SetActive(isTouch);
        
        #endregion
    }

    #endregion

    #region Pause Menu

    private bool _isPaused;

    [SerializeField, TabGroup("⏸", TextColor = "yellow")] private RectTransform _pauseRectTransform;
    [SerializeField, TabGroup("⏸")] private RectTransform _pauseTextTransform;
    
    void Update()
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
        
        _pauseTween = _pauseRectTransform.DOAnchorPosY(Vector3.zero.y, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
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

        _pauseTween = _pauseRectTransform.DOAnchorPosY(1425, 0.2f).SetEase(Ease.OutBack);
        _pauseTextTween = _pauseTextTransform.DORotate(new Vector3(0, -10, -5), 0.2f).SetEase(Ease.OutBack);
    }

    #endregion
}
