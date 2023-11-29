using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ProjectMultiplayer.Connection;
using Fusion;

public class TruckFill : LevelObjective
{
    [SerializeField] private int _boxCapacity;
    [SerializeField] private string _objectTag;
    [SerializeField] private UnityEvent _onObjectiveMeet;
    private int _currentCount;
    public override bool CheckObjective()
    {
        if (_currentCount >= _boxCapacity)
        {
            _onObjectiveMeet?.Invoke();
            NetworkManagerReference.Instance.NetworkRunner.SetActiveScene(levelToLoadWhenObjectiveComplete);
        }
        return _currentCount >= _boxCapacity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_objectTag))
        {
            _currentCount++;
            CheckObjective();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_objectTag))
        {
            _currentCount--;
        }
    }

}
