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
    public enum PositiveEffects { BetterCamera, StopCharacterMovement, GrowSearchable }
    public enum NegativeEffects { FasterCharacters, Rain, ShrinkCharacters }
    
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
                AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[2]);
                break;
            case TypeOfCharEffect.BadEffect:
                gridToUse = _badEffectsGrid;
                effectToUse = _badEffectPrefab;
                AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[3]);
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
            OnEffectDone?.Invoke();
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
    [TabGroup("🏃"), SerializeField] 
    private string _speedEffectDescription;
    public string SpeedEffectDescription => _speedEffectDescription;
    [TabGroup("🏃"), SerializeField] 
    private string _speedEffectName;
    public string SpeedEffectName => _speedEffectName;
    
    private Tween _speedTween;
    
    [Button, TabGroup("🏃")]
    public void FasterCharacters()
    {
        _speedTween?.Kill();
        
        OnSpeedChange?.Invoke(_highSpeed);
        _speedTween = DOVirtual.DelayedCall( _speedEffectDuration, () =>
        {
            OnSpeedChange?.Invoke(NormalSpeed);
        }).SetUpdate(false);
    }
    
    #endregion
    
    #region Better Camera
    
    [field:SerializeField, TabGroup("🎥")] 
    public Sprite BetterCameraIcon { get; private set; }
    [TabGroup("🎥"), SerializeField] 
    private float _cameraEffectDuration = 5;
    public float CameraEffectDuration => _cameraEffectDuration;
    [TabGroup("🎥"), SerializeField] 
    private string _betterCameraDescription;
    public string BetterCameraDescription => _betterCameraDescription;
    [TabGroup("🎥"), SerializeField] 
    private string _betterCameraEffectName;
    public string BetterCameraEffectName => _betterCameraEffectName;
    
    private Tween _cameraTween;
    
    [Button, TabGroup("🎥")]
    public void BetterCamera()
    {
        _cameraTween?.Kill();
        
        CinemachineCamerasHandler.Instance.SwitchCam(CinemachineCamerasHandler.CameraState.FollowSearchableCam);
        _cameraTween = DOVirtual.DelayedCall( _cameraEffectDuration, () =>
        {
            CinemachineCamerasHandler.Instance.SwitchCam();
        }).SetUpdate(false);
    }
    
    #endregion

    #region Stop Character Movement

    public static event Action<bool> OnStopMovement;
    
    [field:SerializeField, TabGroup("🗿")] 
    public Sprite StopCharacterMoveIcon { get; private set; }
    [TabGroup("🗿"), SerializeField] 
    private float _stopEffectDuration = 10;
    public float StopEffectDuration => _stopEffectDuration;
    [TabGroup("🗿"), SerializeField] 
    private string _freezeEffectDescription;
    public string FreezeEffectDescription => _freezeEffectDescription;
    [TabGroup("🗿"), SerializeField] 
    private string _freezeEffectName;
    public string FreezeEffectName => _freezeEffectName;
    
    private Tween _stopTween;

    [Button, TabGroup("🗿")]
    public void StopCharacterMovement()
    {
        _stopTween?.Kill();
        
        OnStopMovement?.Invoke(false);
        _stopTween = DOVirtual.DelayedCall( _stopEffectDuration, () =>
        {
            OnStopMovement?.Invoke(true);
        }).SetUpdate(false);
    }

    #endregion
    
    #region Rain
    
    [field:SerializeField, TabGroup("☔")] 
    public Sprite RainIcon { get; private set; }
    [TabGroup("☔"), SerializeField] 
    private float _rainEffectDuration = 10;
    [TabGroup("☔"), SerializeField] 
    private ParticleSystem _rainEffect;
    public float RainEffectDuration => _rainEffectDuration;
    [TabGroup("☔"), SerializeField] 
    private string _rainEffectDescription;
    public string RainEffectDescription => _rainEffectDescription;
    [TabGroup("☔"), SerializeField] 
    private string _rainEffectName;
    public string RainEffectName => _rainEffectName;
    
    [Button, TabGroup("☔")]
    public void StartRain()
    {
        var particles = Instantiate(_rainEffect);
        var main = particles.main;
        main.duration = _rainEffectDuration;
        particles.Play();
        Destroy(particles.gameObject, main.duration + 2);
    }
    
    #endregion

    #region Shrink Characters

    public static event Action<Vector3> OnShrinkCharacters;
    
    [field:SerializeField, TabGroup("📉")] 
    public Sprite ShrinkCharacterIcon { get; private set; }

    [TabGroup("📉"), SerializeField] 
    private float _shrinkSize;
    [TabGroup("📉"), SerializeField] 
    private float _shrinkEffectDuration = 10;
    public float ShrinkEffectDuration => _shrinkEffectDuration;
    [TabGroup("📉"), SerializeField] 
    private string _shrinkEffectDescription;
    public string ShrinkEffectDescription => _shrinkEffectDescription;
    [TabGroup("📉"), SerializeField] 
    private string _shrinkEffectName;
    public string ShrinkEffectName => _shrinkEffectName;
    
    private Tween _shrinkTween;

    [Button, TabGroup("📉")]
    public void ShrinkCharacters()
    {
        _shrinkTween?.Kill();
        
        OnShrinkCharacters?.Invoke(_shrinkSize * Vector3.one);
        _shrinkTween = DOVirtual.DelayedCall( _shrinkEffectDuration, () =>
        {
            OnShrinkCharacters?.Invoke(Vector3.one);
        }).SetUpdate(false);
    }

    #endregion
    
    #region Grow Searchable
    
    private void OnEnable()
    {
        Searchables.OnSearchableChanged += ChangeSearchableCharacter; 
    }
    
    [field:SerializeField, TabGroup("📈")] 
    public Sprite GrowSearchableIcon { get; private set; }
    [TabGroup("📈"), SerializeField] 
    private float _growSize;
    public float GrowSize => _growSize;
    [TabGroup("📈"), SerializeField] 
    private float _growEffectDuration = 10;
    public float GrowEffectDuration => _growEffectDuration;
    [TabGroup("📈"), SerializeField] 
    private string _growEffectDescription;
    public string GrowEffectDescription => _growEffectDescription;
    [TabGroup("📈"), SerializeField] 
    private string _growEffectName;
    public string GrowEffectName => _growEffectName;
    
    private Tween _growTween;
    private Transform _searchableVisualTransform;
    
    [Button, TabGroup("📈")]
    public void GrowSearchable()
    {
        if(_growTween != null && _growTween.IsActive() && !_growTween.IsPlaying()) _growTween?.Kill();
        
        _searchableVisualTransform = Searchables.Instance.ActiveSearchable.transform.GetChild(0);
        _searchableVisualTransform.DOScale(Vector3.one * GrowSize, 0.2f);
        _growTween = DOVirtual.DelayedCall(_growEffectDuration, () =>
        {
            _searchableVisualTransform.transform.DOScale(Vector3.one, 0.2f);
        }).SetUpdate(false);
    }

    private void ChangeSearchableCharacter()
    {
        if (_growTween == null || !_growTween.IsActive() || !_growTween.IsPlaying()) return;
        _searchableVisualTransform = Searchables.Instance.ActiveSearchable.transform.GetChild(0);
        _searchableVisualTransform.DOScale(Vector3.one * GrowSize, 0.2f);
    }

    #endregion
    
    
}
