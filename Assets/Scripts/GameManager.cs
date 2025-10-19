using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
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
        GameObject character = characters[Random.Range(0, characters.Length)];
        for (int i = 0; i < 3; i++)
        {
            GameObject charaRef = Instantiate(character, GetRandomScreenPosition(), Quaternion.identity);
            charaRef.GetComponent<SearchableCharacter>().ActiveSearchable = true;
            
            GameObject[] otherCharacters = System.Array.FindAll(characters, c => c != character);

            Instantiate(otherCharacters[Random.Range(0, otherCharacters.Length)], GetRandomScreenPosition(), Quaternion.identity);
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
