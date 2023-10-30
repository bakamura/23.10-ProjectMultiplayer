using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeInputPlayer : MonoBehaviour
{
    private static InitializeInputPlayer _instance;
    public static InitializeInputPlayer Instance
    {
        get
        {
            // se ainda n tiver uma referência da instancia, procura ela no GameObject
            if(_instance == null)
            {
                InitializeInputPlayer[] results = GameObject.FindObjectsOfType<InitializeInputPlayer>();
                if (results.Length > 0)
                {
                    if (results.Length > 1) Debug.Log($"Multiple Instances of {typeof(InitializeInputPlayer).Name} found, destroing extras");
                    for (int i = 1; i < results.Length; i++)
                    {
                        Destroy(results[i]);
                    }
                    _instance = results[0];
                }
            }
            // se ainda n tiver uma referência da instancia, cria uma do tipo desejado
            if (_instance == null)
                _instance = new GameObject($"Instance of Type: {typeof(InitializeInputPlayer)}").AddComponent<InitializeInputPlayer>();
            return _instance;
        }
    }
    private InputPlayer _inputActions;
    public InputPlayer PlayerActions
    {
        get 
        {
            if (_inputActions == null)  return _inputActions = new InputPlayer();            
            return _inputActions;
        }
    }

    private void Awake()
    {
        PlayerActions.Enable();
         if(_instance != this) Destroy(this);
    }
}
