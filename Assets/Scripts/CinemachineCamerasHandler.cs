using System;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class CinemachineCamerasHandler : MonoBehaviour
{
    #region Singleton
    public static CinemachineCamerasHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
    }
    #endregion

    [SerializeField] private CinemachineCamera _generalCam;
    public CinemachineCamera FollowSearchableCam;
    
    public enum CameraState
    {
        GeneralCam,
        FollowSearchableCam
    }

    private void OnEnable()
    {
        Searchables.OnSearchableChanged += ChangeFollowTarget;
    }
    private void OnDisable()
    {
        Searchables.OnSearchableChanged -= ChangeFollowTarget;
    }
    
    private void ChangeFollowTarget()
    {
        FollowSearchableCam.Follow = Searchables.Instance.ActiveSearchable.transform;
    }
    
    public void SwitchCam(CameraState state = CameraState.GeneralCam)
    {
        switch (state)
        {
            case CameraState.GeneralCam:
                _generalCam.Priority = 1;
                FollowSearchableCam.Priority = 0;
                break;
            case CameraState.FollowSearchableCam:
                _generalCam.Priority = 0;
                FollowSearchableCam.Priority = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
