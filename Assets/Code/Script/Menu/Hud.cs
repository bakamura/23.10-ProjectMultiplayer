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

        private bool _isPaused;

        private void Toggle(InputAction.CallbackContext ctx)
        {
            if(ctx.ReadValue<float>() == 1 && _canvasTransitionCoroutine == null)
            {
                if (_isPaused) ChangeCurrentCanvas(_hudUI);                
                else ChangeCurrentCanvas(_settingsUI);                
                _isPaused = !_isPaused;
            }
        }

        private void OnEnable()
        {
            InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed += Toggle;
        }

        private void OnDisable()
        {
            InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed -= Toggle;
        }
    }
}