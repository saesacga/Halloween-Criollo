using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitApp : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.Quit();
    }

    private Tween _scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        
        _scaleTween = transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        
        _scaleTween = transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}
