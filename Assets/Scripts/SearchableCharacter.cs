using Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;

public class SearchableCharacter : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public bool ActiveSearchable { get; set; }
    private FollowerEntity _followerEntity;
    private Camera _mainCamera;
    
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
        ActiveSearchable = true;
        FollowSearchable.Instance.Target = transform;
        
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
        Vector3 randomPos = GetRandomPositionOnScreen();
        
        _followerEntity.destination = randomPos;
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
}
