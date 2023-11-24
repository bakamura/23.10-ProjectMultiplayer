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
        [SerializeField, Min(0)] private float _travelDistance;
        [SerializeField] private float _speed;
        private Vector3 _initialPosition;
        private bool _isMoving;
        private bool _isActive;
        private sbyte _directionSign;

        public void Activate()
        {
            if (!_isMoving && !_isActive)
            {
                _directionSign = 1;
                _initialPosition = _movablePart.position;
                _isMoving = true;
            }
        }

        public void Deactivate()
        {
            if (!_isMoving && _isActive)
            {
                _directionSign = -1;
                _initialPosition = _movablePart.position;
                _isMoving = true;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_isMoving)
            {
                _movablePart.position += _speed * _directionSign * Runner.DeltaTime * transform.forward;
                _middlePart.localScale += _speed * _directionSign * Runner.DeltaTime * transform.forward;
                if (Vector3.Distance(_movablePart.position, _initialPosition) >= _travelDistance)
                {
                    _isActive = _directionSign > 0;
                    _isMoving = false;
                }
            }
        }

    }
}