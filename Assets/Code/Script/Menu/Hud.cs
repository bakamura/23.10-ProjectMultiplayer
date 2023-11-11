using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.Player;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ProjectMultiplayer.UI
{
    public class Hud : Menu
    {
        [SerializeField] private CanvasGroup _settingsUI;
        [SerializeField] private CanvasGroup _hudUI;

        //private bool _isPaused;

        private InputAction _return;

        protected override void Awake()
        {
            base.Awake();
            _return = InitializeInputPlayer.Instance.PlayerActions.actions["Cancel"];
            LoadControlBindings();
        }

        private void Update()
        {
            if (_return.WasPressedThisFrame())
            {
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