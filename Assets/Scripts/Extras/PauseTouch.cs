using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseTouch : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Sprite _playSprite;
    private Image _buttonImage;
    private bool _isPaused;

    private void Awake()
    {
        _buttonImage = GetComponent<Image>();
        _buttonImage.sprite = _pauseSprite;
    }
    
    private Tween _scaleTween;
    public void OnPointerClick(PointerEventData eventData)
    {
        _isPaused = !_isPaused;
        
        _scaleTween?.Kill();

        _scaleTween = transform.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBack).SetUpdate(true)
            .OnComplete(() =>
            {
                _scaleTween = transform.DOScale(Vector3.one, 0.1f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
            });
        
        _buttonImage.sprite = _isPaused ? _playSprite : _pauseSprite;
        
        UIManager.Instance.TogglePauseMenu();
    }
}

