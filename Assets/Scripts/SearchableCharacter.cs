using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SearchableCharacter : MonoBehaviour, IPointerClickHandler
{
    public bool ActiveSearchable { get; set; }
    private FollowerEntity _followerEntity;
    private Camera _mainCamera;
    
    [SerializeField] private float _padding = 0.5f;
    
    
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
    
    void Update()
    {
        if (!_followerEntity.hasPath)
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
        var x = Random.Range(_padding, 1f - _padding);
        var y = Random.Range(_padding, 1f - _padding);

        Vector3 worldPos = _mainCamera.ViewportToWorldPoint(new Vector3(x, y, 0));
        worldPos.z = 0;
        return worldPos;
    }
}
