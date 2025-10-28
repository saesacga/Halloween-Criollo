using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpComingUI : MonoBehaviour
{
    #region Singleton 
    public static PowerUpComingUI Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion
    
    
    [TabGroup("References"), SerializeField] private SpriteRenderer _characterRT;
    [TabGroup("References"), SerializeField] private Animator _animatorRT;
    [TabGroup("References"), SerializeField] private TextMeshProUGUI _charText;
    [TabGroup("References"), SerializeField] private RectTransform _characterPowerUpUI;
    [TabGroup("References"), SerializeField] private Image _timer;
    
    [TabGroup("Params"), SerializeField] private float _timerDuration = 5;
    private Sequence _sequence;
    
    public static event Action OnPowerUpTimerUp;

    private void OnEnable()
    {
        UIManager.OnEndLevelUIOpen += HideCharacterPowerUpUI;
    }
    private void OnDisable()
    {
        UIManager.OnEndLevelUIOpen -= HideCharacterPowerUpUI;
    }

    public void SetSprite(Material mat, RuntimeAnimatorController animController, string charName)
    {
        _characterRT.material = mat;
        _animatorRT.runtimeAnimatorController = animController;
        _charText.text = charName;
        
        _sequence?.Kill();
        _timer.fillAmount = 1f;
        _sequence = DOTween.Sequence();
        
        _sequence.Append(_characterPowerUpUI.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _sequence.Join(_charText.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _sequence.Append(_timer.rectTransform.DOScale(0.5f, 0.2f).SetEase(Ease.OutBounce)
            .OnComplete(()=>AudioManager.Instance.PlayLoop(AudioManager.Instance.SfxClips[7])));
        _sequence.Append(DOTween.To(() => 1f, h => _timer.fillAmount = h, 0f, _timerDuration).SetEase(Ease.Linear)).OnComplete(()=>
        {
            HideCharacterPowerUpUI();
            OnPowerUpTimerUp?.Invoke();
        });
    }
    
    
    public void HideCharacterPowerUpUI()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        AudioManager.Instance.StopLoop();
        
        _sequence.Append(_timer.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBounce));
        _sequence.Join(_charText.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        _sequence.Append(_characterPowerUpUI.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
    }
}
