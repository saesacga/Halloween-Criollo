using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using Sirenix.OdinInspector;

public class GameTime : MonoBehaviour
{
    #region Singleton 
    public static GameTime Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion
    
    private readonly float _gameEndHour = 23f;   // 11 PM
    private readonly float _gameStartHour = 19f; // 7 PM
    [SerializeField] private float _durationInSeconds = 180f;
    public float CurrentHour { get; private set; }

    [SerializeField] private TextMeshProUGUI _clockDisplay;
    
    public static event Action OnTimeEnd;
    
    void Start()
    { 
        SetTime();
    }

    private Tween _timeTween;
    public void SetTime()
    {
        float totalHours = _gameEndHour - _gameStartHour;
        float t = 0;

        _timeTween = DOTween.To(() => t, x => t = x, totalHours, _durationInSeconds)
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                CurrentHour = _gameStartHour + t;
                UpdateClockDisplay();
            })
            .OnComplete(() => {
                OnTimeEnd?.Invoke();
            });
    }

    private int _lastMinute = -1;
    void UpdateClockDisplay()
    {
        int hour = Mathf.FloorToInt(CurrentHour);
        int minute = Mathf.FloorToInt((CurrentHour - hour) * 60f);
        
        string ampm = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;
        
        string formattedTime = $"{displayHour:D2}:{minute:D2} {ampm}";

        if (_clockDisplay != null)
        { 
            _clockDisplay.text = formattedTime; 
            if (minute != _lastMinute)
            {
                _clockDisplay.transform.DOKill(); 
                _clockDisplay.transform.localScale = Vector3.one; 
                _clockDisplay.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);

                _lastMinute = minute;
            }
        }
    }
    
    [Button]
    private void PauseTimer()
    {
        _timeTween?.Pause();
    }
    [Button]
    private void ResumeTimer()
    {
        if (_timeTween == null) SetTime();
     
        if(_timeTween.IsPlaying()) return;
        
        _timeTween.Play();
    }
}