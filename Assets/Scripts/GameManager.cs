using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Singleton y Object Pool
    public static GameManager Instance { get; private set; }
    public ObjectPool<GameObject> Pool;
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;

        Pool = new ObjectPool<GameObject>(createFunc: () => null,
            actionOnGet: OnGet,
            actionOnRelease: obj => { if(obj != null) obj.SetActive(false); },
            actionOnDestroy: obj => { if(obj != null) Destroy(obj); },
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: _numberOfCharacters);
    }

    private void OnGet(GameObject obj)
    {
        if (obj == null) return;
        
        obj.transform.position = _spawnPos[Random.Range(0, _spawnPos.Length)].position;
        
        obj.SetActive(true);
    }

    #endregion
    
    [SerializeField] private GameObject[] _characterPrefabs;
    [SerializeField] private int _numberOfCharacters = 10;
    [SerializeField] private Transform[] _corners;
    [SerializeField] private float _spawnRadius = 3f;
    public float SpawnRadius => _spawnRadius;
    
    [SerializeField] private Transform[] _spawnPos;
    
    [SerializeField] private Material[] _paletteMaterials;
    private readonly Dictionary<GameObject, Material> _reservedMaterial = new();

    private GameObject _charaRef;
    
    private void Start()
    {
        for (var i = 0; i < _numberOfCharacters; i++)
        {
            #region SpawnPoint

            int index = i % _corners.Length;
            
            #endregion

            var prefabToUse = _characterPrefabs[Random.Range(0, _characterPrefabs.Length)];
             
            _charaRef = Instantiate(prefabToUse, _spawnPos[Random.Range(0, _spawnPos.Length)].position, Quaternion.identity);
            
            _charaRef.GetComponent<SearchableCharacter>().Corner = _corners[index];
            _charaRef.GetComponent<SearchableCharacter>().SetMaterial(AssignMaterial(prefabToUse));

            if (i >= 1) continue;
            _charaRef.GetComponent<SearchableCharacter>().SetSearchable();
        }
    }
    
    private List<GameObject> _searchableCharactersList = new List<GameObject>();
    private Material AssignMaterial(GameObject prefabType)
    {
        Material chosenMat = null;

        if (_reservedMaterial.TryGetValue(prefabType, out var value)) //There is a reserved material
        {
            var allowedMats = _paletteMaterials.Where(m => m != value).ToArray();
            chosenMat = allowedMats[Random.Range(0, allowedMats.Length)];
            
            Pool.Release(_charaRef);
        }
        else
        {
            chosenMat = _paletteMaterials[Random.Range(0, _paletteMaterials.Length)];
            _reservedMaterial[prefabType] = chosenMat;
            
            _searchableCharactersList.Add(_charaRef);
            if (_searchableCharactersList.Count > 1) _charaRef.SetActive(false); 
        }
        
        return chosenMat;
    }

    private int _currentIndex;
    public void ChangeSearchableCharacter()
    {
        if (_searchableCharactersList.Count == 0) return;
        
        _currentIndex = (_currentIndex + 1) % _searchableCharactersList.Count; //Cycles through the list
        
        var nextCharacter = _searchableCharactersList[_currentIndex];

        if (!nextCharacter.activeSelf) nextCharacter.SetActive(true);
        nextCharacter.GetComponent<SearchableCharacter>().SetSearchable();
        
        ActivateObjects(500);
    }

    public void ActivateObjects(int count)
    {
        for (var i = 0; i < count; i++)
        { 
            Pool.Get();
        }
    }
}
