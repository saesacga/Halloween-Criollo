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
    [TabGroup("Game Parameters", TextColor = "orange"), EnumToggleButtons, OnValueChanged(nameof(UpdateEntitySpawnQuantity)), SerializeField]
    private Level _currentLevel;
    public Level CurrentLevel
    {
        get => _currentLevel; // devuelve el valor actual
        set
        {
            if (_currentLevel == value) return;
            _currentLevel = value;
            UpdateEntitySpawnQuantity();
        }
    }
    
    [TabGroup("Game Parameters"), ShowIf("@_currentLevel == Level.One"), SerializeField]
    private int _numberOfTotalCharacters1, _spawnPerClick1;

    [TabGroup("Game Parameters"), ShowIf("@_currentLevel == Level.Two"), SerializeField]
    private int _numberOfTotalCharacters2, _spawnPerClick2;

    [TabGroup("Game Parameters"), ShowIf("@_currentLevel == Level.Three"), SerializeField]
    private int _numberOfTotalCharacters3, _spawnPerClick3;
    
    [TabGroup("Game Parameters"), InfoBox("Walk radius of Agents around the corner points"), SerializeField] 
    private float _walkRadius;
    public float WalkRadius => _walkRadius;
    
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
    public int SpawnPerClick => _spawnPerClick;
    
    private GameObject _charaRef;
    public static event Action OnCharactersInstantiated;
    
    #endregion

    private void OnEnable()
    {
        GameTime.OnTimeEnd += LevelEnd;
    }

    private void OnDisable()
    {
        GameTime.OnTimeEnd -= LevelEnd;
    }
    
    private void Start()
    {
        UpdateEntitySpawnQuantity();
        
        InstantiateCharacters();
    }

    private void InstantiateCharacters()
    {
        for (var i = 0; i < NumberOfCharacters; i++)
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

    private void LevelEnd()
    {
        UnsearchablePool.Instance.DestroyAll();
        _reservedMaterial.Clear();
        
        CurrentLevel = (Level)(((int)CurrentLevel + 1) % Enum.GetValues(typeof(Level)).Length);
        
        InstantiateCharacters();
        GameTime.Instance.SetTime();
    }

    private void UpdateEntitySpawnQuantity()
    {
        _numberOfCharacters = _currentLevel switch
        {
            Level.One => _numberOfTotalCharacters1,
            Level.Two => _numberOfTotalCharacters2,
            Level.Three => _numberOfTotalCharacters3,
            _ => throw new ArgumentOutOfRangeException()
        };

        _spawnPerClick = _currentLevel switch
        {
            Level.One => _spawnPerClick1,
            Level.Two => _spawnPerClick2,
            Level.Three => _spawnPerClick3,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
