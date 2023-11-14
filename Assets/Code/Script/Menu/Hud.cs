using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.Player;
using UnityEngine.InputSystem;
using TMPro;
using ProjectMultiplayer.Connection;

namespace ProjectMultiplayer.UI
{
    public class Hud : Menu
    {
        [Header("Canvas References")]
        [SerializeField] private CanvasGroup _settingsUI;
        [SerializeField] private CanvasGroup _hudUI;
        [SerializeField] private IconData[] _iconsData;

        [Header("Inputs Display Components")]
        [SerializeField] private TMP_Text[] _pauseTexts;
        [SerializeField] private TMP_Text _action1Text;
        [SerializeField] private TMP_Text _action2Text;
        [SerializeField] private TMP_Text _action3Text;

        [Header("Test")]
        [SerializeField] private TMP_Text _action1Name;
        [SerializeField] private TMP_Text _action2Name;
        [SerializeField] private TMP_Text _action3Name;
        //private bool _isPaused;

        private InputAction _return;

        [System.Serializable]
        private struct IconData
        {
            public NetworkManager.PlayerType PlayerType;
            public string TextAction1;
            public string TextAction2;
            public string TextAction3;
        }

        protected override void Awake()
        {
            base.Awake();
            _onTransitionEnd += UpdateMouseDisplay;
            Cursor.lockState = CursorLockMode.Locked;
            LoadControlBindings();
            _return = InitializeInputPlayer.Instance.PlayerActions.actions["Cancel"];
            UpdateKeyDisplay();
        }

        private void Start()
        {
            StartCoroutine(UpdateActionsIcons());
        }

        private void OnEnable()
        {
            _return.Enable();
        }

        private void OnDisable()
        {
            _return.Disable();
        }

        private void Update()
        {
            if (_return.WasPressedThisFrame())
            {
                GetPreviousCanvasGroup(out CanvasGroup temp);
                if (temp == _hudUI) UpdateKeyDisplay();
                if (!temp)
                {
                    //_isPaused = true;
                    ChangeCurrentCanvas(_settingsUI);
                }
                else
                {
                    //_isPaused = false;
                    ChangeCurrentCanvas(temp);
                }
            }
        }      

        private void UpdateMouseDisplay(CanvasGroup currentCanvas)
        {
            if (currentCanvas == _hudUI)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }               

        private IEnumerator UpdateActionsIcons()
        {
            while (!CanUpdateIcons()) yield return null;
        }

        private bool CanUpdateIcons()
        {
            Player.Player[] players = FindObjectsOfType<Player.Player>();
            if (players != null && players.Length > 0)
            {
                NetworkManager.PlayerType temp = NetworkManagerReference.Instance.PlayersDictionary[NetworkManagerReference.LocalPlayerIDInServer].PlayerType;
                for (int i = 0; i < _iconsData.Length; i++)
                {
                    if (_iconsData[i].PlayerType == temp)
                    {
                        _action1Name.text = _iconsData[i].TextAction1;
                        _action2Name.text = _iconsData[i].TextAction2;
                        _action3Name.text = _iconsData[i].TextAction3;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private void UpdateKeyDisplay()
        {
            for (int i = 0; i < _pauseTexts.Length; i++)
            {
                _pauseTexts[i].text = InitializeInputPlayer.Instance.PlayerActions.actions["Cancel"].GetBindingDisplayString();
            }
            _action1Text.text = InitializeInputPlayer.Instance.PlayerActions.actions["Action1"].GetBindingDisplayString();
            _action2Text.text = InitializeInputPlayer.Instance.PlayerActions.actions["Action2"].GetBindingDisplayString();
            _action3Text.text = InitializeInputPlayer.Instance.PlayerActions.actions["Action3"].GetBindingDisplayString();
        }

        public void SaveControlBindings()
        {
            var rebinds = InitializeInputPlayer.Instance.PlayerActions.actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", rebinds);
        }

        private void LoadControlBindings()
        {
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                InitializeInputPlayer.Instance.PlayerActions.actions.LoadBindingOverridesFromJson(rebinds);
        }
    }
}