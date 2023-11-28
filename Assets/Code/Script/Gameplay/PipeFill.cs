using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using ProjectMultiplayer.Connection;

public class PipeFill : LevelObjective
{
    [SerializeField] private int _orderMax;
    [SerializeField] private string _objectTag;
    [SerializeField] private UnityEvent _onObjectiveMeet;
    private int _currentCount;

    public override bool CheckObjective()
    {
        if (_currentCount >= _orderMax)
        {
            _onObjectiveMeet?.Invoke();
            NetworkManagerReference.Instance.NetworkRunner.SetActiveScene(levelToLoadWhenObjectiveComplete);
        }
        return _currentCount >= _orderMax;
    }

    public void IncreaseObjective()
    {
        _currentCount++;
        CheckObjective();
    }

    public void TryDecreaseObjective()
    {
        _currentCount--;
    }
}
