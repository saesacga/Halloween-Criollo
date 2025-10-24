using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EffectsManager : MonoBehaviour
{
    #region Singleton 
    public static EffectsManager Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion

    #region Setup
    
    public enum TypeOfCharEffect { GoodEffect, BadEffect }
    public enum PositiveEffects { BetterCamera, StopCharacterMovement }
    public enum NegativeEffects { FasterCharacters }
    
    public static event Action OnEffectGiven;
    public static event Action OnEffectDone;

    [TabGroup("Setup"), Title("Logic"), SerializeField]
    private RectTransform _goodEffectsGrid;
    [TabGroup("Setup"), SerializeField]
    private RectTransform _badEffectsGrid;
    [TabGroup("Setup"), SerializeField]
    private GameObject _effectSlotPrefab;
    
    [TabGroup("Setup"), Title("Visuals"), SerializeField]
    private RectTransform _visualEffectsParent;
    [TabGroup("Setup"), SerializeField]
    private GameObject _goodEffectPrefab;
    [TabGroup("Setup"), SerializeField]
    private GameObject _badEffectPrefab;

    private Tween _tween;
    public void GiveEffectToPlayer(Transform caller, TypeOfCharEffect typeOfCharEffect)
    {
        if (_tween != null && _tween.IsActive() && _tween.IsPlaying()) return;

        RectTransform gridToUse;
        GameObject effectToUse;
        switch (typeOfCharEffect)
        {
            case TypeOfCharEffect.GoodEffect:
                gridToUse = _goodEffectsGrid;
                effectToUse = _goodEffectPrefab;
                break;
            case TypeOfCharEffect.BadEffect:
                gridToUse = _badEffectsGrid;
                effectToUse = _badEffectPrefab;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeOfCharEffect), typeOfCharEffect, null);
        }
        
        var slot = Instantiate(_effectSlotPrefab, gridToUse);
        var effect = Instantiate(effectToUse, _visualEffectsParent);

        if (Camera.main != null)
        {
            var screenPos = Camera.main.WorldToScreenPoint(caller.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _visualEffectsParent,
                screenPos,
                _visualEffectsParent.GetComponentInParent<Canvas>().worldCamera,
                out var canvasPos
            );
            effect.GetComponent<RectTransform>().anchoredPosition = canvasPos;
        } //Set effect position relative to the gifter
        
        _tween = DOVirtual.DelayedCall( 0.1f, () =>
        {
            effect.GetComponent<EffectUIButton>().SlotTransform = slot.GetComponent<RectTransform>();
            OnEffectGiven?.Invoke();
        })
        .OnKill(() => _tween = null);
    }

    public void TriggerOnEffectDone()
    {
        OnEffectDone?.Invoke();
    }

    #endregion

    #region Faster Characters

    public float NormalSpeed { get; set; }
    public static event Action<float> OnSpeedChange;
    
    [field:SerializeField, TabGroup("🏃")] 
    public Sprite HighSpeedIcon { get; private set; }
    [TabGroup("🏃"), SerializeField] 
    private float _highSpeed = 20;
    [TabGroup("🏃"), SerializeField] 
    private float _speedEffectDuration = 5;
    public float SpeedEffectDuration => _speedEffectDuration;
    
    private Tween _speedTween;
    
    [Button, TabGroup("🏃")]
    public void FasterCharacters()
    {
        _speedTween?.Kill();
        
        OnSpeedChange?.Invoke(_highSpeed);
        _speedTween = DOVirtual.DelayedCall( _speedEffectDuration, () =>
        {
            OnSpeedChange?.Invoke(NormalSpeed);
        });
    }
    
    #endregion
    
    #region Better Camera
    
    [field:SerializeField, TabGroup("🎥")] 
    public Sprite BetterCameraIcon { get; private set; }
    [TabGroup("🎥"), SerializeField] 
    private float _cameraEffectDuration = 5;
    public float CameraEffectDuration => _cameraEffectDuration;
    
    private Tween _cameraTween;
    
    [Button, TabGroup("🎥")]
    public void BetterCamera()
    {
        _cameraTween?.Kill();
        
        CinemachineCamerasHandler.Instance.SwitchCam(CinemachineCamerasHandler.CameraState.FollowSearchableCam);
        _cameraTween = DOVirtual.DelayedCall( _cameraEffectDuration, () =>
        {
            CinemachineCamerasHandler.Instance.SwitchCam();
        });
    }
    
    #endregion

    #region Stop Character Movement

    public static event Action<bool> OnStopMovement;
    
    [field:SerializeField, TabGroup("🗿")] 
    public Sprite StopCharacterMoveIcon { get; private set; }
    [TabGroup("🗿"), SerializeField] 
    private float _stopEffectDuration = 10;
    public float StopEffectDuration => _stopEffectDuration;
    
    private Tween _stopTween;

    [Button, TabGroup("🗿")]
    public void StopCharacterMovement()
    {
        _stopTween?.Kill();
        
        OnStopMovement?.Invoke(true);
        _stopTween = DOVirtual.DelayedCall( _stopEffectDuration, () =>
        {
            OnStopMovement?.Invoke(false);
        });
    }

    #endregion

}
