using System;
using Pathfinding;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
    private FollowerEntity _followerEntity;

    private void OnEnable()
    {
        EffectsManager.OnSpeedChange += ChangeSpeed;
        
        _followerEntity = GetComponent<FollowerEntity>();
        EffectsManager.Instance.NormalSpeed = _followerEntity.maxSpeed;
    }
    private void OnDisable()
    {
        EffectsManager.OnSpeedChange -= ChangeSpeed;
    }
    
    private void ChangeSpeed(float speed)
    {
        _followerEntity.maxSpeed = speed;
    }
}
