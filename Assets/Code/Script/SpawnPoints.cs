using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private Vector3[] _spawnPoints;
    [Header("Debug")]
    [SerializeField] private bool _debugMode;
    [SerializeField] private Color _debugColor;
    [SerializeField] private float _debugGizmosSize;    

    public Vector3 GetRandomSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length - 1)];
    }

    private void OnDrawGizmosSelected()
    {
        if (_debugMode)
        {
            Gizmos.color = _debugColor;
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + _spawnPoints[i], _debugGizmosSize);
            }
        }
    }
}
