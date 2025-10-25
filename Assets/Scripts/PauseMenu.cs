using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private bool _isPaused;

    private RectTransform _rectTransform;
    private void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
        
        if (!_isPaused)
            PauseGame();
            
        else 
            ResumeGame();
    }
    
    private Tween _pauseTween;
    private void PauseGame()
    {
        _pauseTween?.Kill();
        
        AudioManager.Instance.PauseAllAudio();
        Time.timeScale = 0f;
        
        _pauseTween = _rectTransform.DOAnchorPosY(Vector3.zero.y, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
        
        _isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.ResumeAllAudio();
        
        _isPaused = false;
        _pauseTween?.Kill();
        _pauseTween = _rectTransform.DOAnchorPosY(1425, 0.2f).SetEase(Ease.OutBack);
    }
}
