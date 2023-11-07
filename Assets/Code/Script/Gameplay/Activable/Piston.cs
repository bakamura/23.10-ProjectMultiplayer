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
        [SerializeField, Min(0)] private float _travelDistance;
        [SerializeField] private float _speed;
        private Vector3 _initialPosition;
        private bool _isMoving;
        private sbyte _directionSign;
        public void Activate()
        {
            _directionSign = 1;
            _initialPosition = _movablePart.position;
            _isMoving = true;
        }

        public void Deactivate()
        {
            _directionSign = -1;
            _initialPosition = _movablePart.position;
            _isMoving = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (_isMoving)
            {
                _movablePart.position += _speed * _directionSign * Runner.DeltaTime * transform.forward;
                if(Vector3.Distance(_movablePart.position, _initialPosition) >= _travelDistance)
                {
                    _isMoving = false;
                }
            }
        }

    }
}