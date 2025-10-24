using UnityEngine;
using Pathfinding;

public class VisualAgent : MonoBehaviour
{
   private FollowerEntity _followerEntity;
   
   private void OnEnable() 
   { 
       _followerEntity = GetComponentInParent<FollowerEntity>();
   }
   
   private void Update() 
   { 
       var velocity = _followerEntity.velocity;
        
       if (Mathf.Abs(velocity.x) > 0.05f)
       { 
           transform.localScale = new Vector3( velocity.x < 0 ? -1 : 1, 
               1, 
               1
               );
       } 
   }
}
