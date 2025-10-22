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

    #region Faster Characters

    public float NormalSpeed { get; set; }
    public static event Action<float> OnSpeedChange;
    
    [TabGroup("🏃"), SerializeField] 
    private float _highSpeed = 20;
    [TabGroup("🏃"), SerializeField] 
    private float _speedEffectDuration = 5;
    
    [Button, TabGroup("🏃")]
    public void FasterCharacters()
    {
        OnSpeedChange?.Invoke(_highSpeed);
        DOVirtual.DelayedCall( _speedEffectDuration, () =>
        {
            OnSpeedChange?.Invoke(NormalSpeed);
        });
    }
    
    #endregion
    
    #region Better Camera
    
    [TabGroup("🎥"), PropertyRange(2, 5), SerializeField]
    private float _cameraDistance;
    [TabGroup("🎥"), SerializeField] 
    private float _cameraEffectDuration = 5;
    
    [Button, TabGroup("🎥")]
    public void BetterCamera()
    {
        CinemachineCamerasHandler.Instance.FollowSearchableCam.Lens.OrthographicSize = _cameraDistance;
        
        CinemachineCamerasHandler.Instance.SwitchCam(CinemachineCamerasHandler.CameraState.FollowSearchableCam);
        DOVirtual.DelayedCall( _cameraEffectDuration, () =>
        {
            CinemachineCamerasHandler.Instance.SwitchCam();
        });
    }
    
    #endregion
    
}
