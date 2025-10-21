using System;
using UnityEngine;

public class FollowSearchable : MonoBehaviour
{
    #region Singleton 
    public static FollowSearchable Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
        
        _characterRT = transform.GetChild(0).GetComponent<SpriteRenderer>();
    } 
    #endregion
    
    private SpriteRenderer _characterRT;
    
    public void SetSprite(Sprite sprite, Material mat)
    {
        _characterRT.sprite = sprite;
        _characterRT.material = mat;
    }
}
