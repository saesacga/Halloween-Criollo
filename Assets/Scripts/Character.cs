using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected string CharacterName;
    [field: SerializeField, ReadOnly] public bool ActiveSearchable { get; set; }
    [SerializeField] private bool _useCorner;

    private FollowerEntity _followerEntity;
    private Camera _mainCamera;
    public Transform Corner { get; set; }
    
    protected GameObject VisualCharacter;
    
    protected virtual void OnEnable()
    {
        _mainCamera = Camera.main;
        _followerEntity = GetComponent<FollowerEntity>();
        VisualCharacter = transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        SetNewRandomDestination();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ActiveSearchable) SetUnsearchable();
        else WrongAnimation();
    }
    
    public void SetSearchable()
    {
        ActiveSearchable = true;
        
        _useCorner = false;
        _followerEntity.rvoSettings.priority = 1;
        
        VisualCharacter.transform.localPosition = Vector3.back;
        
        SetSearchableUI();
    }
    protected virtual void SetSearchableUI()
    {
        var spriteRenderer = VisualCharacter.GetComponent<SpriteRenderer>();
        FollowSearchable.Instance.SetSprite(spriteRenderer.material, VisualCharacter.GetComponent<Animator>().runtimeAnimatorController);//For camera Render Texture
        
        GameManager.Instance.CharacterName.text = CharacterName;
    }
    
    protected void SetUnsearchable()
    {
        GameManager.Instance.UpdateScore();
        
        ActiveSearchable = false;
        
        _useCorner = true;
        _followerEntity.rvoSettings.priority = 0;
        
        VisualCharacter.transform.localPosition = Vector3.zero;
        RightAnimation();
        UnsearchablePolymorph();
    }
    protected virtual void UnsearchablePolymorph()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[0], 2f);
        Searchables.Instance.ChangeSearchableCharacter();
    }

    #region Visuals

    private Tween _shakeTween;
    private void WrongAnimation()
    {
        _shakeTween?.Kill();
        
        AudioManager.Instance.PlaySfx(AudioManager.Instance.SfxClips[1]);
        
        _shakeTween = VisualCharacter.transform.DOShakePosition(
            duration: 0.3f,
            strength: new Vector3(0.05f, 0.05f, 0),
            vibrato: 30,
            randomness: 90,
            snapping: false, 
            fadeOut: false
        );
    }

    private Tween _scaleTween;
    private void RightAnimation()
    {
        var particles = Instantiate(GameManager.Instance.Particles[Random.Range(0, GameManager.Instance.Particles.Length)], transform);
        particles.transform.localPosition = new Vector3(0, 0.5f, 0);
        particles.Play();
        Destroy(particles.gameObject, particles.main.duration);
        
        _scaleTween?.Kill();
        
        _scaleTween= VisualCharacter.transform.DOScale(1.5f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                VisualCharacter.transform.DOScale(1, 0.2f)
                    .SetEase(Ease.OutBack);
            });
    }

    public void SetMaterial(Material mat)
    {
        VisualCharacter.GetComponent<SpriteRenderer>().material = mat;
    }
    
    #endregion

    #region Pathfinding
    
    private void Update()
    {
        if (_followerEntity.reachedEndOfPath || _followerEntity.velocity.sqrMagnitude < 0.1f)
        {
            SetNewRandomDestination();
        }
        
        var velocity = _followerEntity.velocity;
        
        if (Mathf.Abs(velocity.x) > 0.05f)
        { 
            transform.localScale = new Vector3( velocity.x < 0 ? -1 : 1, 
                1, 
                1
            );
        }
    }

    void SetNewRandomDestination()
    {
        if (_useCorner && Corner != null)
        {
            _followerEntity.destination = CornerPosition();
        }
        else
        {
            _followerEntity.destination = GetRandomPositionOnScreen();
        }
        
    }

    Vector3 GetRandomPositionOnScreen()
    {
        Vector3 randomPos;
        float minDistance = 2.5f;
        int attempts = 0;

        do
        {
            Vector2 randomViewportPoint = new Vector2(
                Random.Range(0f, 1f),
                Random.Range(0f, 0.9f)
            );

            randomPos = _mainCamera.ViewportToWorldPoint(
                new Vector3(randomViewportPoint.x, randomViewportPoint.y, _mainCamera.nearClipPlane)
            );
            randomPos.z = transform.position.z;

            attempts++;
        }
        while (Vector3.Distance(transform.position, randomPos) < minDistance && attempts < 10);

        return randomPos;
    }

    private Vector3 CornerPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * GameManager.Instance.WalkRadius;
        Vector3 targetPos = Corner.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        return targetPos;
    }

    #endregion
}
