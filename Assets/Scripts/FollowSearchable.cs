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
    } 
    #endregion
    
    public Transform Target { get; set; }
    [SerializeField] private float _yOffset = 1f;

    private void Update()
    {
        if (Target != null)
        {
            transform.position = new Vector3(Target.position.x, Target.position.y + _yOffset, -10f);
        }
    }
}
