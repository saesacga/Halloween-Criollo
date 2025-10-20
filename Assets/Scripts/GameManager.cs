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
    
    private void Start()
    {
        GameObject character = _characterPrefabs[Random.Range(0, _characterPrefabs.Length)];
        GameObject[] otherCharacters = System.Array.FindAll(_characterPrefabs, c => c != character);

        for (var i = 0; i < _numberOfCharacters; i++)
        {
            int cornerIndex = i % _corners.Length;
            Transform assignedCorner = _corners[cornerIndex];
            
            Vector2 randomOffset = Random.insideUnitCircle * _spawnRadius;
            Vector3 spawnPos = assignedCorner.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            
            if (i < 1)
            {
                GameObject charaRef = Instantiate(character);
                charaRef.GetComponent<SearchableCharacter>().SetSearchable(); 
            }
            else
            { 
                GameObject charaRef = Instantiate(otherCharacters[Random.Range(0, otherCharacters.Length)], spawnPos, Quaternion.identity);
                charaRef.GetComponent<SearchableCharacter>().Corner = assignedCorner;
            }
        }
    }
}
