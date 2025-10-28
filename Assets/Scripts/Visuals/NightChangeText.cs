using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class NightChangeText : MonoBehaviour
{
    [SerializeField, Multiline(3)]
    private string _night1Text;
    [SerializeField, Multiline(3)]
    private string _night2Text;
    [SerializeField, Multiline(3)]
    private string _night3Text;
    private TextMeshProUGUI _text;
    
    private void OnEnable()
    {
        UIManager.OnEndLevelUIClose += ShowDate;
        _text = GetComponent<TextMeshProUGUI>();
    }

    private Tween _tween; 
    public void ShowDate()
    {
        _tween?.Kill();
        _text.alpha = 0;
        
        _text.text = GameManager.Instance.CurrentLevel switch
        {
            GameManager.Level.One => _night1Text,
            GameManager.Level.Two => _night2Text,
            GameManager.Level.Three => _night3Text,
            GameManager.Level.Chaos => $"<size=40>????</size>\nCaos Noche {GameManager.Instance.ChaosNight}\n<size=30><color=green>11 disfraces por encontrar</size>",
            _ => throw new ArgumentOutOfRangeException()
        };

        _tween = _text
            .DOFade(1, 2f)
            .SetEase(Ease.Linear)
            .SetDelay(0.3f)
            .SetLoops(2, LoopType.Yoyo);
    }
}
