using System;
using System.Collections.Generic;
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

    private GameObject _activeSearchable;
    public GameObject ActiveSearchable
    {
        get => _activeSearchable;
        private set
        {
            _activeSearchable = value;
            CinemachineCamerasHandler.Instance.FollowSearchableCam.Follow = value.transform;
        }
    }
    
    private void OnEnable()
    {
        GameManager.OnCharactersInstantiated += SetFirstSearchable;
    }
    private void OnDisable()
    {
        GameManager.OnCharactersInstantiated -= SetFirstSearchable;
    }
    
    private readonly List<GameObject> _searchableCharacters = new List<GameObject>();
    private int _currentIndex;
    private void SetFirstSearchable()
    {
        #region Destroy All Searchable Characters (If any)

        if (_searchableCharacters.Count > 0)
        {
            foreach (var searchableCharacter in _searchableCharacters) { searchableCharacter.transform.SetParent(null); }
            foreach (var searchableCharacter in _searchableCharacters) { Destroy(searchableCharacter); }
            _searchableCharacters.Clear();
        }

        #endregion

        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            _searchableCharacters.Add(transform.GetChild(i).gameObject);
        }
        
        _currentIndex = 0;
        
        ChangeSearchableCharacter();
    }
    public void ChangeSearchableCharacter()
    {
        if (transform.childCount == 0) return;
        
        _currentIndex = (_currentIndex + 1) % transform.childCount; //Cycles through the list

        ActiveSearchable = transform.GetChild(_currentIndex).gameObject;
        
        var nextCharacter = ActiveSearchable.GetComponent<Character>();

        if (!nextCharacter.gameObject.activeSelf) nextCharacter.gameObject.SetActive(true); 
        
        nextCharacter.SetSearchable();
        
        UnsearchablePool.Instance.ActivateObjects(GameManager.Instance.SpawnPerClick);
    }
}
