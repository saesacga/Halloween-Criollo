using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

public class LevelCompleteAnimation : MonoBehaviour, IPointerClickHandler
{
    #region Singleton
    public static LevelCompleteAnimation Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
    }

    #endregion

    #region Animation Params
    
    [SerializeField] private float _animationDuration = 1f;

    #endregion
    
    #region UI Scene References
    
    [SerializeField] private RectTransform _levelCompleted;
    
    [SerializeField] private RectTransform _charactersFounded;
   
    [SerializeField] private RectTransform _charactersFoundedNumber;
    
    [SerializeField] private RectTransform _clickToContinue;
    
    [SerializeField] private RectTransform _bg;
    
    #endregion
    
    private TextMeshProUGUI _levelCompletedText;
    private TextMeshProUGUI _charactersFoundedNumberText;

    private Sequence _sequence;

    private void Start()
    {
        _levelCompletedText = _levelCompleted.GetComponent<TextMeshProUGUI>();
        _charactersFoundedNumberText = _charactersFoundedNumber.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            SetLevelCompletedText();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }

    public void SetLevelCompletedText()
    {
        _sequence = DOTween.Sequence();
        
        _levelCompleted.DORotate(new Vector3(0, 10, 5), _animationDuration / 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        
        _sequence.Append(_bg.DOAnchorPos(Vector2.zero, _animationDuration).SetEase(Ease.OutBounce));
        _sequence.Append(_levelCompleted.DOScale(Vector3.one, _animationDuration/2).SetEase(Ease.OutBack));
        //_sequence.Append(_charactersFounded.DOAnchorPos(Vector2.zero, _animationDuration / 2)).SetEase(Ease.OutElastic);
    }
}