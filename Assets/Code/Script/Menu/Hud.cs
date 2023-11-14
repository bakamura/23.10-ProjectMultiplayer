using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.Player;
using UnityEngine.InputSystem;
using TMPro;

namespace ProjectMultiplayer.UI
{
    public class Hud : Menu
    {
        [Header("Canvas References")]
        [SerializeField] private CanvasGroup _settingsUI;
        [SerializeField] private CanvasGroup _hudUI;

        [Header("Inputs Display Components")]
        [SerializeField] private TMP_Text[] _pauseTexts;
        [SerializeField] private TMP_Text _action1Text;
        [SerializeField] private TMP_Text _action2Text;
        [SerializeField] private TMP_Text _action3Text;
        //private bool _isPaused;

        private InputAction _return;

        protected override void Awake()
        {
            base.Awake();
            LoadControlBindings();
            UpdateKeyDisplay();
            _return = InitializeInputPlayer.Instance.PlayerActions.actions["Cancel"];
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
                Debug.Log("input");
                GetPreviousCanvasGroup(out CanvasGroup temp);
                if (temp == _hudUI) UpdateKeyDisplay();
                if(!temp)
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
        
        private void UpdateKeyDisplay()
        {
            for(int i = 0; i < _pauseTexts.Length; i++)
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