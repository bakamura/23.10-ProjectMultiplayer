using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectMultiplayer.Connection;
using TMPro;

namespace ProjectMultiplayer.UI
{
    public class CharacterSelection : MonoBehaviour
    {
        //private CanvasGroup _canvasGroup;
        //{
        //    get
        //    {
        //        if (!_canvasGroup)
        //        {
        //            _canvasGroup = GetComponent<CanvasGroup>();
        //        }
        //        return _canvasGroup;
        //    }
        //    set { }
        //}
        [SerializeField] private PlayerOptionButton[] _characterOptions;
        [SerializeField] private PlayerSelectorData[] _playersSelectorsVisual = new PlayerSelectorData[NetworkManager.MaxPlayerCount];
        //private PlayerOptionButton _currentlySelected;

        [System.Serializable]
        private struct PlayerSelectorData
        {
            public CanvasGroup CanvasGroup;
            public RectTransform RectTransform;
            public GameObject Text;
        }

        private void Awake()
        {
            //_canvasGroup = GetComponent<CanvasGroup>();
            UpdatePlayerSelectionScript temp = FindObjectOfType<UpdatePlayerSelectionScript>();
            for (int i = 0; i < _characterOptions.Length; i++)
            {
                _characterOptions[i].UpdatePlayerSelectionScript = temp;
                _characterOptions[i].Button.onClick.AddListener(_characterOptions[i].UpdateCharacterSelected);
            }
        }

        //public void UpdateSelectedPlayer(int playerIndex, NetworkManager.PlayerType playerType)
        //{
        //    if (_currentlySelected) _currentlySelected.Image.color = Color.white;
        //    _characterOptions[(int)playerType].Image.color = Color.green;
        //    _currentlySelected = _characterOptions[(int)playerType];
        //}

        //public void SetIsInteractable(bool isInteractable)
        //{
        //    _canvasGroup.interactable = isInteractable;
        //}

        public void DeactivatePlayerSelectorVisual(int playerIndex)
        {
            _playersSelectorsVisual[playerIndex].CanvasGroup.alpha = 0;
            _playersSelectorsVisual[playerIndex].RectTransform.position = _characterOptions[0].RectTransform.position;
            _playersSelectorsVisual[playerIndex].Text.SetActive(false);
        }

        public void ActivatePlayerSelectorVisual(int playerIndex, NetworkManager.PlayerType playerType, bool isLocalPlayer)
        {
            _playersSelectorsVisual[playerIndex].CanvasGroup.alpha = 1;
            for (int i = 0; i < _characterOptions.Length; i++)
            {
                if (_characterOptions[i].PlayerType == playerType)
                {
                    _playersSelectorsVisual[playerIndex].RectTransform.position = _characterOptions[i].RectTransform.position;
                    _playersSelectorsVisual[playerIndex].Text.SetActive(isLocalPlayer);
                    break;
                }
            }
        }
    }
}