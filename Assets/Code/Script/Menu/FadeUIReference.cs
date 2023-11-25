using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectMultiplayer.UI
{
    public class FadeUIReference : MonoBehaviour
    {
        private static FadeUI _instance;

        public static FadeUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    FadeUI[] results = GameObject.FindObjectsOfType<FadeUI>();
                    if (results.Length > 0)
                    {
                        if (results.Length > 1) Debug.Log($"Multiple Instances of {typeof(FadeUI).Name} found, destroing extras");
                        for (int i = 1; i < results.Length; i++)
                        {
                            Destroy(results[i]);
                        }
                        _instance = results[0];
                        //_instance.Spawned();
                    }
                }
                return _instance;
            }
        }
    }
}
