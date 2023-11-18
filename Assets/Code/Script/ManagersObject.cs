using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
