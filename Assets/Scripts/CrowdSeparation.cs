using Pathfinding;
using UnityEngine;

public class CrowdSeparation : MonoBehaviour
{
    [SerializeField] private LayerMask _agentLayer;                 

    private FollowerEntity _follower;

    void Start()
    {
        _follower = GetComponent<FollowerEntity>();
    }
}