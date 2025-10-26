using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnsearchablePool : MonoBehaviour
{
    #region Singleton & Object Pool

    public ObjectPool<GameObject> Pool;
    
    private List<GameObject> _activePoolObjects = new List<GameObject>();
    public IReadOnlyList<GameObject> ActivePoolObjects => _activePoolObjects;
    public static UnsearchablePool Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
        
        Pool = new ObjectPool<GameObject>(createFunc: () => null,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: obj => { if(obj != null) Destroy(obj); },
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 800);
    }
    private void OnGet(GameObject obj)
    {
        if (obj == null) return;
        
        obj.transform.position = GameManager.Instance.SpawnPoints[Random.Range(0, GameManager.Instance.SpawnPoints.Length)].position;
        
        obj.SetActive(true);
        _activePoolObjects.Add(obj);
    }

    private void OnRelease(GameObject obj)
    {
        if (obj == null) return;
        
        obj.SetActive(false);
        _activePoolObjects.Remove(obj);
    }
    
    #endregion
    
    private void OnEnable()
    {
        GameManager.OnCharactersInstantiated += PoolComplete;
    }
    private void OnDisable()
    {
        GameManager.OnCharactersInstantiated -= PoolComplete;
    }
    
    private void PoolComplete()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Pool.Release(transform.GetChild(i).gameObject);
        }
    }
    
    public void ActivateObjects(int count)
    {
        for (var i = 0; i < count; i++)
        { 
            Pool.Get();
        }
    }
    
    public void DestroyAll()
    {
        foreach (var obj in _activePoolObjects.ToList()) { Pool.Release(obj); }
        Pool.Clear();
    }
}
