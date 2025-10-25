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
    [SerializeField] private bool _useTime;
    [SerializeField] private float _durationInSeconds = 180f;
    private float _currentHour;

    [SerializeField] private TextMeshProUGUI _clockDisplay;
    
    public static event Action OnTimeEnd;
    public static event Action OnTimeForPowerUp;
    
    void Start()
    { 
        if(_useTime) SetTime();
    }

    private Tween _timeTween;
    public void SetTime()
    {
        var startHour = _gameStartHour;
        var endHour = _gameEndHour;
        var totalHours = endHour - startHour;
        float t = 0;
        
        _timeTween = DOTween.To(() => t, x =>
        {
            t = x;
            _currentHour = startHour + t;
            UpdateClockDisplay();
        }, totalHours, _durationInSeconds).SetEase(Ease.Linear).SetUpdate(false);
        
        var firstCallbackHour = GameManager.Instance.CurrentLevel == GameManager.Level.Chaos ? 19.25f : 19.8f;
        var intervalSeconds = GameManager.Instance.CurrentLevel == GameManager.Level.Chaos? 20f : 45f;
        var elapsedTime = 0f;
        
        var firstCallbackTime = (firstCallbackHour - startHour) / totalHours * _durationInSeconds;

        elapsedTime = firstCallbackTime;
        while (elapsedTime < _durationInSeconds)
        {
            var callbackTime = elapsedTime;
            DOVirtual.DelayedCall(callbackTime, () => OnTimeForPowerUp?.Invoke());
            elapsedTime += intervalSeconds;
        }

        _timeTween.OnComplete(() => OnTimeEnd?.Invoke());
    }


    private int _lastMinute = -1;
    void UpdateClockDisplay()
    {
        int hour = Mathf.FloorToInt(_currentHour);
        int minute = Mathf.FloorToInt((_currentHour - hour) * 60f);
        
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
    public void PauseTimer()
    {
        _timeTween?.Pause();
    }
    [Button]
    public void ResumeTimer()
    {
        if (_timeTween == null) SetTime();
     
        if(_timeTween.IsPlaying()) return;
        
        _timeTween.Play();
    }
}