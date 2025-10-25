using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.EventSystems;

public class SetMusicVolume : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Slider _slider;
    private Tween _sliderTween;
    private RectTransform _knob;
    private Tween _hoverTween;
    
    private enum SliderType { Music, Sfx }
    [EnumButtons, SerializeField]
    private SliderType _sliderType;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _knob = _slider.handleRect;

        _slider.value = _sliderType switch
        {
            SliderType.Music => AudioManager.Instance.MusicVolume,
            SliderType.Sfx => AudioManager.Instance.SfxVolume,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        DOGetter<float> getter;
        DOSetter<float> setter;

        switch (_sliderType)
        {
            case SliderType.Music:
                getter = () => AudioManager.Instance.MusicVolume;
                setter = x => AudioManager.Instance.MusicVolume = x;
                break;
            case SliderType.Sfx:
                getter = () => AudioManager.Instance.SfxVolume;
                setter = x => AudioManager.Instance.SfxVolume = x;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        DOTween.To(getter, setter, value, 1.5f).SetEase(Ease.OutCubic).SetUpdate(false);
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverTween?.Kill();
        _hoverTween = _knob.DOScale(Vector3.one * 1.3f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverTween?.Kill();
        _hoverTween = _knob.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}
