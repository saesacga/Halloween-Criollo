using System;
using UnityEngine.Pool;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class UnsearchablePool : MonoBehaviour
{
    #region Singleton & Object Pool

    private ObjectPool<GameObject> _pool;
    public static UnsearchablePool Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
        
        _pool = new ObjectPool<GameObject>(createFunc: () => null,
            actionOnGet: OnGet,
            actionOnRelease: obj => { if(obj != null) obj.SetActive(false); },
            actionOnDestroy: obj => { if(obj != null) Destroy(obj); },
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 500);
    }
    private void OnGet(GameObject obj)
    {
        if (obj == null) return;
        
        obj.transform.position = GameManager.Instance.SpawnPoints[Random.Range(0, GameManager.Instance.SpawnPoints.Length)].position;
        
        obj.SetActive(true);
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
            _pool.Release(transform.GetChild(i).gameObject);
        }
    }
    
    public void ActivateObjects(int count)
    {
        for (var i = 0; i < count; i++)
        { 
            _pool.Get();
        }
    }
    
    public void DestroyAll()
    {
        _pool.Clear();
    }

    private void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var obj = transform.GetChild(i).gameObject;
                if (obj.activeSelf) _pool.Release(obj);
            }
        }
    }
}
