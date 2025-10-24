using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class NightChangeText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    private void OnEnable()
    {
        LevelCompleteAnimation.OnEndLevelUIClose += ShowDate;
        _text = GetComponent<TextMeshProUGUI>();
    }

    private Tween _tween; 
    public void ShowDate()
    {
        _tween?.Kill();
        _text.text = GameManager.Instance.CurrentLevel switch
        {
            GameManager.Level.One => "Noche 1, 29 de Octubre",
            GameManager.Level.Two => "Noche 2, 30 de Octubre",
            GameManager.Level.Three => "Noche 3, 31 de Octubre",
            _ => throw new ArgumentOutOfRangeException()
        };

        _tween = _text
            .DOFade(1, 2f)
            .SetEase(Ease.Linear)
            .SetDelay(0.3f)
            .SetLoops(2, LoopType.Yoyo);
    }
}
