using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

    #region Game Parameters
    
    public enum Level { One, Two, Three }
    [TabGroup("Game Parameters", TextColor = "orange"), EnumToggleButtons, OnValueChanged(nameof(UpdateEntitySpawn)), SerializeField]
    private Level _currentLevel;
    public Level CurrentLevel
    {
        get => _currentLevel; // devuelve el valor actual
        set
        {
            if (_currentLevel == value) return;
            _currentLevel = value;
            UpdateEntitySpawn();
        }
    }
    
    [TabGroup("Game Parameters"), ShowIf("@_currentLevel == Level.One"), SerializeField]
    private int _numberOfTotalCharactersLevel1, _spawnPerClickLevel1;

    [TabGroup("Game Parameters"), ShowIf("@_currentLevel == Level.Two"), SerializeField]
    private int _numberOfTotalCharactersLevel2, _spawnPerClickLevel2;

    [TabGroup("Game Parameters"), ShowIf("@_currentLevel == Level.Three"), SerializeField]
    private int _numberOfTotalCharactersLevel3, _spawnPerClickLevel3;
    
    #endregion
    
    #region Global Assets References

    [TabGroup("Global Assets References", TextColor = "green"), SerializeField, AssetsOnly] 
    private GameObject[] _characterPrefabs;
    
    [TabGroup("Global Assets References"), SerializeField, AssetsOnly] 
    private Material[] _paletteMaterials;
    private readonly Dictionary<GameObject, Material> _reservedMaterial = new();
    
    #endregion

    #region Global Scene References

    [TabGroup("Global Scene References", TextColor = "yellow"), SerializeField] 
    private Transform[] _corners;
    
    [TabGroup("Global Scene References"), SerializeField] 
    private Transform[] _spawnPoints;
    public Transform[] SpawnPoints => _spawnPoints;
    
    [TabGroup("Global Scene References"), Title("Character Parents"), SerializeField] 
    private Transform _searchableParent;
    [TabGroup("Global Scene References"), SerializeField] 
    private Transform _unsearchableParent;
    
    #endregion

    #region Non Serialized
    
    private int _numberOfCharacters, _spawnPerClick;
    public int NumberOfCharacters => _numberOfCharacters;
    
    private GameObject _charaRef;
    public static event Action OnCharactersInstantiated;
    
    #endregion
    
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

    private void UpdateEntitySpawn()
    {
        _numberOfCharacters = _currentLevel switch
        {
            Level.One => _numberOfTotalCharactersLevel1,
            Level.Two => _numberOfTotalCharactersLevel2,
            Level.Three => _numberOfTotalCharactersLevel3,
            _ => throw new ArgumentOutOfRangeException()
        };

        _spawnPerClick = _currentLevel switch
        {
            Level.One => _spawnPerClickLevel1,
            Level.Two => _spawnPerClickLevel2,
            Level.Three => _spawnPerClickLevel3,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
