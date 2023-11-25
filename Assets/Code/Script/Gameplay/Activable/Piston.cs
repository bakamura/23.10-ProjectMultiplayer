using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{
    public class Piston : NetworkBehaviour, IActivable
    {
        [SerializeField] private Transform _movablePart;
        [SerializeField] private Transform _middlePart;
        [SerializeField, Tooltip("Thte travel distance is always relative to the current position")] private PistonPositionData[] _positionsData;
        [SerializeField] private float _speed;
        private Vector3 _initialPosition;
        private bool _isMoving;
        //private bool _isActive;
        private sbyte _directionSign;
        private sbyte _currentActivations;
        private PistonPositionData _currentPosition;

        [System.Serializable]
        private struct PistonPositionData
        {
            [Min(1f)] public sbyte ActivationsRequired;
            [Min(0f)] public float TravelDistance;
        }

#if UNITY_EDITOR
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private Color _debugColor;
#endif

        public void Activate()
        {
            if (!_isMoving /*&& !_isActive*/)
            {
                _directionSign = 1;
                _initialPosition = _movablePart.position;
                _currentActivations = (sbyte)(_currentActivations < 0 ? 1 : _currentActivations + 1);
                if (GetPositionData()) _isMoving = true;
            }
        }
        
        public void Deactivate()
        {
            if (!_isMoving /*&& _isActive*/)
            {
                _directionSign = -1;
                _initialPosition = _movablePart.position;
                _currentActivations--;              
                if (GetPositionData()) _isMoving = true;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_isMoving)
            {
                _movablePart.position += _speed * _directionSign * Runner.DeltaTime * transform.forward;
                _middlePart.localScale += _speed * _directionSign * Runner.DeltaTime * transform.forward;
                if (Vector3.Distance(_movablePart.position, _initialPosition) >= _currentPosition.TravelDistance)
                {
                    //_isActive = _directionSign > 0;
                    _isMoving = false;
                }
            }
        }

        private bool GetPositionData()
        {
            if (_directionSign > 0)
            {
                for (int i = 0; i < _positionsData.Length; i++)
                {
                    if (_positionsData[i].ActivationsRequired == _currentActivations)
                    {
                        _currentPosition = _positionsData[i];
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _positionsData.Length; i++)
                {
                    if (_positionsData[i].ActivationsRequired == _currentActivations + 1)
                    {
                        _currentPosition = _positionsData[i];
                        return true;
                    }
                }
            }
            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_drawGizmos && _positionsData != null)
            {
                Gizmos.color = _debugColor;
                Vector3 finalPos = _movablePart.position;
                for(int i = 0; i < _positionsData.Length; i++)
                {
                    finalPos += _positionsData[i].TravelDistance * transform.forward;
                    Gizmos.DrawSphere(finalPos, .1f);
                    UnityEditor.Handles.Label(finalPos+ Vector3.up * .2f, i.ToString());
                }
            }
        }
#endif
    }
}