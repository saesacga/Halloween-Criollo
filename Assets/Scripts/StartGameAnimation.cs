using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartGameAnimation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _text1;
    [SerializeField] private TextMeshProUGUI _text2;
    [SerializeField] private TextMeshProUGUI _text3;
    [SerializeField] private TextMeshProUGUI _clickToContinueText;
    [SerializeField] private Image _backgroundImage;
    
    [SerializeField] private NightChangeText _nightChangeText;
    
    private Sequence _sequence;
    private bool _sequenceEnd;
    
    void Start()
    {
        _sequence = DOTween.Sequence();
        
        _sequence.Append(_text1.DOFade(1f, 1.5f));
        _sequence.Append(_text2.DOFade(1f, 3f));
        _sequence.Append(_text3.DOFade(1f, 2f)).SetEase(Ease.Linear).OnComplete(() =>
        {
            _sequenceEnd = true;
            _clickToContinueText.DOFade(0.3f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }

    private void CloseSequence()
    {
        _sequence.Kill();
        _clickToContinueText.DOKill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_clickToContinueText.DOFade(0f, 1f));
        _sequence.Append(_text3.DOFade(0f, 1f));
        _sequence.Append(_text2.DOFade(0f, 1f));
        _sequence.Append(_text1.DOFade(0f, 1f));
        _sequence.Append(_backgroundImage.DOFade(0f, 3f));
        _sequence.OnComplete(() =>
        {
            GameTime.Instance.SetTime();
            _nightChangeText.ShowDate();
            gameObject.SetActive(false);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_sequenceEnd)
        {
            CloseSequence();
        }
    }
}
