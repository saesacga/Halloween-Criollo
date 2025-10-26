using UnityEngine;
using DG.Tweening;

public class FollowSearchable : MonoBehaviour
{
    #region Singleton 
    public static FollowSearchable Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
        
        _characterRT = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
    } 
    #endregion
    
    private SpriteRenderer _characterRT;
    private Animator _animator;
    private Sequence _sequence;
    
    public void SetSprite(Material mat, RuntimeAnimatorController animController)
    {
        if (_sequence != null && _sequence.IsPlaying()) _sequence.Restart();
        
        _sequence = DOTween.Sequence().SetAutoKill(false);
        _sequence.Append(_characterRT.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _characterRT.material = mat;
            _animator.runtimeAnimatorController = animController;
        }));
        _sequence.Append(_characterRT.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }
}
