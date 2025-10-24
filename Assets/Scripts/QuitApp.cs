using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuitApp : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isPaused;

    private RectTransform _rectTransform;
    private void OnEnable()
    {
        _rectTransform = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    private Tween _pauseTween;
    private void PauseGame()
    {
        _pauseTween?.Kill();
        GameTime.Instance.PauseTimer();
        
        _pauseTween = _rectTransform.DOAnchorPosY(Vector3.zero.y, 0.2f).SetEase(Ease.OutBack);
        
        _isPaused = true;
    }

    private void ResumeGame()
    {
        GameTime.Instance.ResumeTimer();
        _isPaused = false;
        _pauseTween?.Kill();
        _pauseTween = _rectTransform.DOAnchorPosY(1425, 0.2f).SetEase(Ease.OutBack);
        
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.Quit();
    }

    private Tween _scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        
        _scaleTween = transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        
        _scaleTween = transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
}
