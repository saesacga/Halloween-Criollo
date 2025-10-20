using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _characterPrefabs;
    [SerializeField] private int _numberOfCharacters = 10;
    [SerializeField] private float _padding = 0.5f;
    
    #region Singleton 
    public static GameManager Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion
        
    private void Start()
    {
        GameObject character = _characterPrefabs[Random.Range(0, _characterPrefabs.Length)];
        GameObject[] otherCharacters = System.Array.FindAll(_characterPrefabs, c => c != character);

        for (var i = 0; i < _numberOfCharacters; i++)
        {
            if (i < 1)
            {
                GameObject charaRef = Instantiate(character);
                charaRef.GetComponent<SearchableCharacter>().SetSearchable(); 
            }
            else
            { 
                Instantiate(otherCharacters[Random.Range(0, otherCharacters.Length)], GetRandomScreenPosition(), Quaternion.identity);
            }
        }
    }
    
    private Vector3 GetRandomScreenPosition()
    {
        //Main Cam
        Camera cam = Camera.main;

        // Limits
        float x = Random.Range(_padding, 1 - _padding);
        float y = Random.Range(_padding, 1 - _padding);

        //Convert to world position
        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(x, y, cam.nearClipPlane + 5f));
        worldPos.z = 0; // Para 2D
        return worldPos;
    }
}
