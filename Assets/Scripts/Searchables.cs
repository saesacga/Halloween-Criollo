using System;
using UnityEngine;

public class Searchables : MonoBehaviour
{
    #region Singleton

    public static Searchables Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
    }
    
    #endregion

    private void OnEnable()
    {
        GameManager.OnCharactersInstantiated += SetFirstSearchable;
    }
    private void OnDisable()
    {
        GameManager.OnCharactersInstantiated -= SetFirstSearchable;
    }

    private void SetFirstSearchable()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        ChangeSearchableCharacter();
    }

    private int _currentIndex;
    public void ChangeSearchableCharacter()
    {
        if (transform.childCount == 0) return;
        
        _currentIndex = (_currentIndex + 1) % transform.childCount; //Cycles through the list
        
        var nextCharacter = transform.GetChild(_currentIndex).GetComponent<Character>();

        nextCharacter.gameObject.SetActive(true);
        nextCharacter.SetSearchable();
        
        UnsearchablePool.Instance.ActivateObjects(500);
    }
}
