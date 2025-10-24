using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
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
    private Level CurrentLevel
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
    private GameObject[] _powerUpCharacterPrefabs;
    
    [TabGroup("Global Assets References"), SerializeField, AssetsOnly] 
    private Material[] _paletteMaterials;
    private readonly Dictionary<GameObject, Material> _reservedMaterial = new();
    
    [TabGroup("Global Assets References"), SerializeField, AssetsOnly] 
    private ParticleSystem[] _particles;
    public ParticleSystem[] Particles => _particles;
    [TabGroup("Global Assets References"), SerializeField, AssetsOnly] 
    private ParticleSystem _badEffectParticles;
    public ParticleSystem BadEffectParticles => _badEffectParticles;
    
    #endregion

    #region Global Scene References

    [TabGroup("Global Scene References", TextColor = "yellow"), SerializeField, SceneObjectsOnly] 
    private Transform[] _corners;
    
    [TabGroup("Global Scene References"), SerializeField, SceneObjectsOnly] 
    private Transform[] _spawnPoints;
    public Transform[] SpawnPoints => _spawnPoints;
    
    [TabGroup("Global Scene References"), Title("Character Parents"), SerializeField, SceneObjectsOnly] 
    private Transform _searchableParent;
    [TabGroup("Global Scene References"), SerializeField, SceneObjectsOnly] 
    private Transform _unsearchableParent;
    [TabGroup("Global Scene References"), SerializeField, SceneObjectsOnly]
    private Transform _powerUpParent;

    [TabGroup("Global Scene References"), Title("In Game UI"), SerializeField, SceneObjectsOnly]
    private TextMeshProUGUI _dailyScoreUI;
    [TabGroup("Global Scene References"), SerializeField, SceneObjectsOnly]
    private TextMeshProUGUI _totalScoreUI;
    [field: SerializeField, TabGroup("Global Scene References"), SceneObjectsOnly]
    public TextMeshProUGUI CharacterName { get; set; }
    
    #endregion

    #region Non Serialized
    
    private int _numberOfCharacters, _spawnPerClick;
    public int NumberOfCharacters => _numberOfCharacters;
    public int SpawnPerClick => _spawnPerClick;

    public int DailyScore { get; set; }

    private int _totalScore;
    
    
    private GameObject _charaRef;
    public static event Action OnCharactersInstantiated;
    
    #endregion

    private void OnEnable()
    {
        GameTime.OnTimeEnd += LevelEnd;
        GameTime.OnTimeForPowerUp += SpawnPowerUp;
        LevelCompleteAnimation.OnEndLevelUIOpen += NewLevelOpenMenuConfig;
        LevelCompleteAnimation.OnEndLevelUIClose += NewLevelCloseMenuConfig;
    }

    private void OnDisable()
    {
        GameTime.OnTimeEnd -= LevelEnd;
        GameTime.OnTimeForPowerUp -= SpawnPowerUp;
        LevelCompleteAnimation.OnEndLevelUIOpen -= NewLevelOpenMenuConfig;
        LevelCompleteAnimation.OnEndLevelUIClose -= NewLevelCloseMenuConfig;
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

    public void UpdateScore()
    {
        DailyScore++;
        _totalScore++;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        _dailyScoreUI.text = $"{DailyScore}";
        _totalScoreUI.text = $"{_totalScore}";
    }

    private void LevelEnd()
    {
        LevelCompleteAnimation.Instance.OpenLevelCompleteUI();
        CurrentLevel = (Level)(((int)CurrentLevel + 1) % Enum.GetValues(typeof(Level)).Length);
        
    }

    private void NewLevelOpenMenuConfig()
    {
        UnsearchablePool.Instance.DestroyAll();
        _reservedMaterial.Clear();
        
        DailyScore = 0;
        UpdateScoreText();

        DOVirtual.DelayedCall(0.1f, () =>
        {
            InstantiateCharacters();
            UnsearchablePool.Instance.ActivateObjects(30);
        });
    }
    private void NewLevelCloseMenuConfig()
    {
        GameTime.Instance.SetTime();
    }

    [TabGroup("Game Parameters"), Button]
    private void SpawnPowerUp()
    {
        var prefabToUse = _powerUpCharacterPrefabs[Random.Range(0, _powerUpCharacterPrefabs.Length)];
        var powerUp = Instantiate(prefabToUse, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
        powerUp.transform.SetParent(_powerUpParent);
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
