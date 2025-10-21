using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SearchableCharacter : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField, ReadOnly] public bool ActiveSearchable { get; set; }
    [SerializeField] private bool _useCorner;
    
    private FollowerEntity _followerEntity;
    private Camera _mainCamera;
    public Transform Corner { get; set; }
    
    private GameObject _visualCharacter;
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ActiveSearchable) SetUnsearchable();
    }

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        
        _followerEntity = GetComponent<FollowerEntity>();
        _visualCharacter = transform.GetChild(0).gameObject;

        SetNewRandomDestination();
    }

    public void SetSearchable()
    {
        ActiveSearchable = true;
        
        _useCorner = false;
        _followerEntity.rvoSettings.priority = 1;
        
        _visualCharacter.transform.localPosition = Vector3.back;
        
        var spriteRenderer = _visualCharacter.GetComponent<SpriteRenderer>();
        FollowSearchable.Instance.SetSprite(spriteRenderer.sprite, spriteRenderer.material);//For camera Render Texture
    }
    private void SetUnsearchable()
    {
        ActiveSearchable = false;
        
        _useCorner = true;
        _followerEntity.rvoSettings.priority = 0;
        
        _visualCharacter.transform.localPosition = Vector3.zero;
        
        GameManager.Instance.ChangeSearchableCharacter();
        
        _visualCharacter.transform.DOScale(1.5f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _visualCharacter.transform.DOScale(1, 0.2f)
                    .SetEase(Ease.OutBack);
            });
    }

    public void SetMaterial(Material mat)
    {
        _visualCharacter.GetComponent<SpriteRenderer>().material = mat;
    }

    private void Update()
    {
        if (_followerEntity.reachedEndOfPath || _followerEntity.velocity.sqrMagnitude < 0.1f)
        {
            SetNewRandomDestination();
        }
        
        if (Keyboard.current.gKey.wasPressedThisFrame && !ActiveSearchable)
        {
            GameManager.Instance.Pool.Release(gameObject);
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
                Random.Range(0f, 1f)
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
        Vector2 randomOffset = Random.insideUnitCircle * GameManager.Instance.SpawnRadius;
        Vector3 targetPos = Corner.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        return targetPos;
    }
}
