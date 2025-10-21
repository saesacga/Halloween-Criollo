using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
    }

    #endregion
    
    [SerializeField] private GameObject[] _characterPrefabs;
    [SerializeField] private int _numberOfCharacters;
    public int NumberOfCharacters => _numberOfCharacters;
    [SerializeField] private Transform[] _corners;
    [SerializeField] private float _spawnRadius;
    public float SpawnRadius => _spawnRadius;
    
    [SerializeField] private Transform[] _spawnPoints;
    public Transform[] SpawnPoints => _spawnPoints;
    
    [SerializeField] private Transform _searchableParent, _unsearchableParent;
    
    [SerializeField] private Material[] _paletteMaterials;
    private readonly Dictionary<GameObject, Material> _reservedMaterial = new();

    private GameObject _charaRef;
    
    public static event Action OnCharactersInstantiated;
    
    private void Start()
    {
        for (var i = 0; i < _numberOfCharacters; i++)
        {
            int index = i % _corners.Length;
            
            var prefabToUse = _characterPrefabs[Random.Range(0, _characterPrefabs.Length)];
             
            _charaRef = Instantiate(prefabToUse, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
            
            _charaRef.GetComponent<Character>().Corner = _corners[index];
            _charaRef.GetComponent<Character>().SetMaterial(UniqueMaterialCheck(prefabToUse));
        }
        
        OnCharactersInstantiated?.Invoke();
    }
    
    private Material UniqueMaterialCheck(GameObject prefabType)
    {
        Material chosenMat = null;

        if (_reservedMaterial.TryGetValue(prefabType, out var value)) //There is a reserved material
        {
            var allowedMats = _paletteMaterials.Where(m => m != value).ToArray();
            chosenMat = allowedMats[Random.Range(0, allowedMats.Length)];
            
            _charaRef.transform.SetParent(_unsearchableParent);
        }
        else
        {
            chosenMat = _paletteMaterials[Random.Range(0, _paletteMaterials.Length)];
            _reservedMaterial[prefabType] = chosenMat;
            
            _charaRef.transform.SetParent(_searchableParent);
        }
        
        return chosenMat;
    }
}
