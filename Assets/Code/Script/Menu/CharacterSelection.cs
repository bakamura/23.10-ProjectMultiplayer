using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    private CanvasGroup _canvasGroup 
    {
        get
        {
            if (!_canvasGroup)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
        set { }
    }
    [SerializeField] private PlayerOptionButton[] _characterOptions;
    private PlayerOptionButton _currentlySelected;

    private void Awake()
    {
        for(int i = 0; i < _characterOptions.Length; i++)
        {
            _characterOptions[i].UpdatePlayerSelectionScript = FindObjectOfType<UpdatePlayerSelectionScript>();
            _characterOptions[i].Button.onClick.AddListener(_characterOptions[i].UpdateCharacterSelected);
        }
    }

    public void UpdateSelectedPlayer(NetworkManager.PlayerType playerType)
    {
        if (_currentlySelected) _currentlySelected.Image.color = Color.white;
        _characterOptions[(int)playerType].Image.color = Color.green;
        _currentlySelected = _characterOptions[(int)playerType];
    }

    public void SetIsInteractable(bool isInteractable)
    {
        _canvasGroup.interactable = isInteractable;
    }    
}
