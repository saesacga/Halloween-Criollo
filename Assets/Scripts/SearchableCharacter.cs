using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SearchableCharacter : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public bool ActiveSearchable { get; set; }
    private FollowerEntity _followerEntity;
    private Camera _mainCamera;
    
    [SerializeField] private float _paddingX = 0.1f;
    [SerializeField] private float _paddingY = 0.2f;
    
    
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
                Random.Range(0f + _paddingX, 1f - _paddingX),
                Random.Range(0f + _paddingY, 1f - _paddingY)
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
