using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField] private int _numberOfCharacters = 10;
    [SerializeField] private Transform[] _corners;
    [SerializeField] private float _spawnRadius = 3f;
    public float SpawnRadius => _spawnRadius;
    
    [SerializeField] private Material[] _paletteMaterials;
    private readonly Dictionary<GameObject, Material> _reservedMaterial = new();

    private GameObject _charaRef;
    
    private void Start()
    {
        for (var i = 0; i < _numberOfCharacters; i++)
        {
            #region SpawnPoint

            int cornerIndex = i % _corners.Length;
            Transform assignedCorner = _corners[cornerIndex];
            
            Vector2 randomOffset = Random.insideUnitCircle * _spawnRadius;
            Vector3 spawnPos = assignedCorner.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            #endregion

            var prefabToUse = _characterPrefabs[Random.Range(0, _characterPrefabs.Length)];
             
            _charaRef = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
            
            _charaRef.GetComponent<SearchableCharacter>().SetMaterial(AssignMaterialWithUniqueFirst(prefabToUse));
            _charaRef.GetComponent<SearchableCharacter>().Corner = assignedCorner;
            
            if (i < 1) _charaRef.GetComponent<SearchableCharacter>().SetSearchable();
        }
    }
    
    private List<GameObject> _searchableCharactersList = new List<GameObject>();
    private Material AssignMaterialWithUniqueFirst(GameObject prefabType)
    {
        Material chosenMat = null;

        if (_reservedMaterial.TryGetValue(prefabType, out var value)) //There is a reserved material
        {
            var allowedMats = _paletteMaterials.Where(m => m != value).ToArray();
            chosenMat = allowedMats[Random.Range(0, allowedMats.Length)];
        }
        else
        {
            chosenMat = _paletteMaterials[Random.Range(0, _paletteMaterials.Length)];
            _reservedMaterial[prefabType] = chosenMat;
            _searchableCharactersList.Add(_charaRef);
        }
        
        return chosenMat;
    }

    private int _currentIndex;
    public void ChangeSearchableCharacter()
    {
        if (_searchableCharactersList.Count == 0) return;
        
        _currentIndex = (_currentIndex + 1) % _searchableCharactersList.Count; //Cycles through the list
        
        var nextCharacter = _searchableCharactersList[_currentIndex];
        nextCharacter.GetComponent<SearchableCharacter>().SetSearchable();
    }
}
