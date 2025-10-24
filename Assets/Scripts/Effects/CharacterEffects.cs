using System;
using Pathfinding;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
    private FollowerEntity _followerEntity;
    private Animator _animator;
    
    private static readonly int Idle = Animator.StringToHash("Idle"); 
    private static readonly int Walk = Animator.StringToHash("Walk");

    private void OnEnable()
    {
        EffectsManager.OnSpeedChange += ChangeSpeed;
        EffectsManager.OnStopMovement += StopMovement;
        
        _followerEntity = GetComponent<FollowerEntity>();
        EffectsManager.Instance.NormalSpeed = _followerEntity.maxSpeed;
        
        _animator = GetComponentInChildren<Animator>();
    }
    private void OnDisable()
    {
        EffectsManager.OnSpeedChange -= ChangeSpeed;
        EffectsManager.OnStopMovement -= StopMovement;
    }
    
    private void ChangeSpeed(float speed)
    {
        _followerEntity.maxSpeed = speed;
    }

    private void StopMovement(bool stop)
    {
        _followerEntity.canMove = stop;
        _animator.Play(stop ? Walk : Idle);
    }
}
