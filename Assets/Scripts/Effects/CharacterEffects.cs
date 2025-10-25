using System;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
    private FollowerEntity _followerEntity;
    private Animator _animator;
    private Transform _visualTransform;
    
    private static readonly int Idle = Animator.StringToHash("Idle"); 
    private static readonly int Walk = Animator.StringToHash("Walk");

    private void OnEnable()
    {
        EffectsManager.OnSpeedChange += ChangeSpeed;
        EffectsManager.OnStopMovement += StopMovement;
        EffectsManager.OnShrinkCharacters += ShrinkCharacter;
        
        _followerEntity = GetComponent<FollowerEntity>();
        EffectsManager.Instance.NormalSpeed = _followerEntity.maxSpeed;
        
        _animator = GetComponentInChildren<Animator>();
        _visualTransform = transform.GetChild(0);
    }
    private void OnDisable()
    {
        EffectsManager.OnSpeedChange -= ChangeSpeed;
        EffectsManager.OnStopMovement -= StopMovement;
        DOTween.Kill(this);
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

    private Tween _scaleTween;
    private void ShrinkCharacter(Vector3 size)
    {
        _scaleTween?.Kill();
        _scaleTween = _visualTransform.DOScale(size, 0.2f).SetEase(Ease.OutBack);
    }
}
