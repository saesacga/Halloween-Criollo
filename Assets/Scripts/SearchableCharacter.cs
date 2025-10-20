using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class SearchableCharacter : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField, ReadOnly] public bool ActiveSearchable { get; set; }
    [SerializeField] private bool _useCorner;
    
    private FollowerEntity _followerEntity;
    private Camera _mainCamera;
    public Transform Corner { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(ActiveSearchable ? "Found!" : "Wrong Character");
    }

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        _followerEntity = GetComponent<FollowerEntity>();
        SetNewRandomDestination();
    }

    public void SetSearchable()
    {
        _useCorner = false;
        
        ActiveSearchable = true;
        FollowSearchable.Instance.Target = transform; //For camera to follow
        
        _followerEntity.rvoSettings.priority = 1;
        
        var charaRefVisual = transform.GetChild(0).gameObject;
        charaRefVisual.layer = LayerMask.NameToLayer("Searchable");
        charaRefVisual.transform.position = Vector3.back;
    }

    private void Update()
    {
        if (_followerEntity.reachedEndOfPath || _followerEntity.velocity.sqrMagnitude < 0.1f)
        {
            SetNewRandomDestination();
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
