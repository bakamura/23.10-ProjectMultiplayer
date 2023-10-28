using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainScreen : Menu {

    [SerializeField] private CanvasGroup _mainMenu;

    private void OnEnable()
    {        
        InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed += ReturnToPreviousCanvas;
    }

    private void OnDisable()
    {
        InitializeInputPlayer.Instance.PlayerActions.UI.Cancel.performed -= ReturnToPreviousCanvas;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ReturnToPreviousCanvas(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValue<float>() == 1 && GetPreviousCanvasGroup(out CanvasGroup temp))
        {
            ChangeCurrentCanvas(temp);
        }
    }
}
